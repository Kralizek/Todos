// Imports System.Diagnostics.CodeAnalysis for attributes like ExcludeFromCodeCoverage.
using System.Diagnostics.CodeAnalysis;

// Imports the namespace for specifications, specifically to use PriorityTodoItemSpecification.
using Todos.Todo.Specifications;

// Defines the namespace for this Endpoints class, grouping it with other Todo-related logic.
namespace Todos.Todo;

// Declares a public static class named 'Endpoints'.
// A static class cannot be instantiated and can only contain static members.
// This class will contain extension methods and static handler methods for defining API endpoints.
public static class Endpoints
{
    // The [ExcludeFromCodeCoverage] attribute tells code coverage tools to ignore this method.
    // This is an extension method for IEndpointRouteBuilder (indicated by 'this IEndpointRouteBuilder builder').
    // Extension methods allow you to add methods to existing types without modifying them.
    // This method is designed to be called from Program.cs (e.g., app.AddTodoEndpoints()) to register all Todo-related API routes.
    // It returns IEndpointRouteBuilder to allow for fluent chaining if needed.
    [ExcludeFromCodeCoverage]
    public static IEndpointRouteBuilder AddTodoEndpoints(this IEndpointRouteBuilder builder)
    {
        // 'builder.MapGroup("/todos")' creates a route group. All endpoints defined within this group
        // will have their routes prefixed with "/todos".
        // '.WithTags("Todos")' adds a tag to these endpoints for Swagger documentation, grouping them in the UI.
        // '.WithOpenApi()' enables OpenAPI (Swagger) metadata generation for these endpoints.
        var group = builder.MapGroup("/todos")
            .WithTags("Todos")
            .WithOpenApi();

        // Maps an HTTP GET request to the root of the group (i.e., "/todos") to the 'List' static method below.
        // '.WithName("ListTodos")' gives a unique name to this endpoint, useful for generating links or for other tools.
        group.MapGet("/", List)
            .WithName("ListTodos");

        // Maps an HTTP POST request to "/todos" to the 'Create' static method.
        group.MapPost("/", Create)
            .WithName("CreateTodo");

        // Maps an HTTP GET request to "/todos/{id}" to the 'Get' static method.
        // '{id:guid:required}' defines a route parameter named 'id'.
        //   - 'guid' constrains the parameter to be a valid GUID.
        //   - 'required' makes this parameter mandatory in the path.
        group.MapGet("/{id:guid:required}", Get)
            .WithName("GetTodo");

        // Maps an HTTP PUT request to "/todos/{id}" to the 'Save' static method (intended for updates).
        group.MapPut("/{id:guid:required}", Save)
            .WithName("SaveTodo");

        // Maps an HTTP DELETE request to "/todos/{id}" to the 'Delete' static method.
        group.MapDelete("/{id:guid:required}", Delete)
            .WithName("DeleteTodo");

        // Returns the original builder, allowing further endpoint mapping outside this group if desired.
        return builder;
    }

    // Static handler method for the GET /todos endpoint.
    // ASP.NET Core Minimal APIs use dependency injection to provide parameters like 'ITodoRepository repository'.
    // 'Priority? priority = null' defines an optional query parameter named 'priority'.
    // If the request URL is /todos?priority=High, 'priority' will be Priority.High. If not provided, it's null.
    // Returns IAsyncEnumerable<TodoItem> to stream todo items asynchronously.
    public static IAsyncEnumerable<TodoItem> List(ITodoRepository repository, Priority? priority = null)
    {
        // Creates a list to hold any specifications that need to be applied.
        var specifications = new List<TodoItemSpecification>();

        // Checks if the optional 'priority' query parameter was provided.
        if (priority.HasValue)
        {
            // If a priority is provided, create a new PriorityTodoItemSpecification
            // (using the concrete class from Specifications/PriorityTodoItemSpecification.cs)
            // and add it to the list of specifications.
            specifications.Add(new PriorityTodoItemSpecification(priority.Value));
        }

        // Calls the ListAsync method on the injected ITodoRepository, passing any specifications.
        // The repository will use these specifications to filter the data at the database level.
        return repository.ListAsync(specifications.ToArray());
    }

    // Static handler method for the POST /todos endpoint.
    // 'TodoItem item' parameter is automatically bound from the JSON request body.
    // 'Results<Created<TodoItem>, BadRequest>' is a type from Minimal APIs that helps define
    // the possible HTTP responses and their types, improving Swagger documentation and type safety.
    public static async Task<Results<Created<TodoItem>, BadRequest>> Create(TodoItem item, ITodoRepository repository, CancellationToken cancellationToken)
    {
        // Basic validation: if the client sends an item with an Id, it's a bad request for a create operation,
        // as the server should generate the Id.
        if (item.Id != Guid.Empty)
        {
            // 'TypedResults.BadRequest()' returns an HTTP 400 Bad Request response.
            return TypedResults.BadRequest();
        }

        // Calls the AddAsync method on the repository to save the new todo item.
        var newItem = await repository.AddAsync(item, cancellationToken);

        // 'TypedResults.Created(location, value)' returns an HTTP 201 Created response.
        // The first argument is the URL where the newly created resource can be found (Location header).
        // The second argument is the created resource itself, which will be serialized in the response body.
        return TypedResults.Created($"/todos/{newItem.Id}", newItem);
    }

    // Static handler method for the GET /todos/{id} endpoint.
    // 'Guid id' is bound from the route parameter.
    public static async Task<Results<Ok<TodoItem>, NotFound>> Get(Guid id, ITodoRepository repository, CancellationToken cancellationToken)
    {
        // Calls GetAsync on the repository to retrieve the todo item by its id.
        var item = await repository.GetAsync(id, cancellationToken);

        // Checks if the item was found.
        if (item is null)
        {
            // If not found, 'TypedResults.NotFound()' returns an HTTP 404 Not Found response.
            return TypedResults.NotFound();
        }

        // If found, 'TypedResults.Ok(item)' returns an HTTP 200 OK response with the item in the body.
        return TypedResults.Ok(item);
    }

    // Static handler method for the PUT /todos/{id} endpoint (for updates).
    public static async Task<Results<Ok<TodoItem>, BadRequest, NotFound>> Save(Guid id, TodoItem item, ITodoRepository repository, CancellationToken cancellationToken)
    {
        // Basic validation: if the item in the request body has an Id, it must match the Id in the route.
        // An Id should be present on the item for an update.
        if (item.Id != Guid.Empty && item.Id != id)
        {
            return TypedResults.BadRequest();
        }
        // Ensure the item being updated has the correct ID from the route parameter, if it was empty.
        item.Id = id; 

        try
        {
            // Calls UpdateAsync on the repository.
            await repository.UpdateAsync(item, cancellationToken);
        }
        catch (ArgumentException)
        {
            // The repository's UpdateAsync throws ArgumentException if the item is not found.
            // Catch this and return an HTTP 404 Not Found response.
            return TypedResults.NotFound();
        }

        // If update is successful, return HTTP 200 OK with the updated item.
        // Note: A common alternative for PUT is to return 204 No Content if the update is successful and no body is needed.
        return TypedResults.Ok(item);
    }

    // Static handler method for the DELETE /todos/{id} endpoint.
    public static async Task<Results<Ok, NotFound>> Delete(Guid id, ITodoRepository repository, CancellationToken cancellationToken)
    {
        try
        {
            // Calls DeleteAsync on the repository.
            await repository.DeleteAsync(id, cancellationToken);
        }
        catch (ArgumentException)
        {
            // The repository's DeleteAsync throws ArgumentException if the item is not found.
            // Catch this and return an HTTP 404 Not Found response.
            return TypedResults.NotFound();
        }

        // If deletion is successful, 'TypedResults.Ok()' returns an HTTP 200 OK response with no body.
        // A common alternative is to return 204 No Content.
        return TypedResults.Ok();
    }
}