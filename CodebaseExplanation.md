# Understanding the Todos Interview Codebase

This document provides an explanation of the "Todos" sample application, with a particular focus on the C# backend. The goal is to help you familiarize yourself with its structure and key concepts, especially if you have limited C# experience.

## Overall Architecture

The application is a simple Todo list manager with:

1.  **Backend:** An API built with ASP.NET Core (using C#).
2.  **Frontend:** A web interface built with React (JavaScript).
3.  **Database:** PostgreSQL, for storing todo items.
4.  **Orchestration:** Can be run using Docker Compose or .NET Aspire (which also uses containers).

The repository is designed to facilitate discussion about code structure and choices.

## Backend (ASP.NET Core & C#)

The backend is located in the `backend/` directory. The main API project is in `backend/API/`.

### 1. Project Entry Point: `Program.cs`

This is the starting point for the ASP.NET Core application. It sets up all the services the application needs and defines how HTTP requests are handled.

**Key C# Concepts & Snippets:**

*   **`var builder = WebApplication.CreateBuilder(args);`**
    *   `var`: This C# keyword declares an implicitly typed local variable. The compiler figures out the type (`WebApplicationBuilder` in this case) from the right side of the assignment.
    *   This line creates a "builder" object used to configure the application.

*   **Service Configuration (Dependency Injection):**
    Services are components that provide functionality to your application. ASP.NET Core has a built-in system for managing these called Dependency Injection (DI).
    ```csharp
    // Example: Adding Swagger (API documentation tool) services
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Todo API",
            Version = new Version(1, 0, 0).ToString()
        });
    });

    // Example: Adding the Database Context (for database operations)
    // "Database" is the name of the connection string in appsettings.json
    builder.AddNpgsqlDbContext<AppDbContext>("Database");

    // Example: Registering our custom TodoRepository
    // This tells the app: "When someone asks for an ITodoRepository,
    // give them an instance of EntityFrameworkTodoRepository."
    builder.Services.AddTransient<ITodoRepository, EntityFrameworkTodoRepository>();
    ```
    *   `builder.Services.Add...()`: These methods register different services.
    *   `AddTransient`: One type of service lifetime. A new instance of `EntityFrameworkTodoRepository` is created each time it's requested.

*   **Middleware Pipeline:**
    After services are configured, the request handling pipeline is built. Middleware are components that process HTTP requests and responses.
    ```csharp
    var app = builder.Build(); // Builds the application

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger(); // Enables Swagger middleware
        app.UseSwaggerUI(); // Enables Swagger interactive UI
    }

    // This line finds and registers our Todo API endpoints (GET, POST, etc.)
    app.AddTodoEndpoints();

    app.Run(); // Starts the application and listens for HTTP requests
    ```

*   **Minimal APIs:**
    The line `app.AddTodoEndpoints();` indicates the use of ASP.NET Core "Minimal APIs." This is a newer, more concise way to define HTTP endpoints directly in `Program.cs` or related files, often without needing full controller classes. We'll see `AddTodoEndpoints` in `Todo/Endpoints.cs`.

### 2. Database Interaction: `AppDbContext.cs`

This file defines the "database context" using Entity Framework Core (EF Core), which is an Object-Relational Mapper (ORM). An ORM lets you interact with your database using C# objects instead of writing raw SQL.

**File Path:** `backend/API/AppDbContext.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using Todos.Todo; // Namespace for TodoItem

namespace Todos; // Namespace for this class

// AppDbContext inherits from DbContext (from Entity Framework Core)
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    // This DbSet represents the 'todos' table in the database.
    // You'll use 'Todos' to query and save TodoItem objects.
    public DbSet<TodoItem> Todos => Set<TodoItem>();

    // This method is used to configure the database model (e.g., table names, constraints)
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Maps the TodoItem class to a table named "todos"
        var todo = modelBuilder.Entity<TodoItem>().ToTable("todos");

        // Sets a maximum length for the Title property
        todo.Property(p => p.Title).HasMaxLength(128);

        base.OnModelCreating(modelBuilder);
    }
}
```

*   **`DbContext`**: A base class from EF Core. Your context class (`AppDbContext`) represents a session with the database.
*   **`DbSet<TodoItem> Todos`**: Represents a collection of `TodoItem` entities. EF Core will map this to a table (named "todos" in this case). You use this property to query (e.g., `context.Todos.ToList()`) and save data.
*   **`OnModelCreating`**: This method is called by EF Core to build the database model. Here, it specifies the table name (`"todos"`) and a maximum length for the `Title` property.
*   **Constructor Injection (`DbContextOptions<AppDbContext> options`)**: The database connection details and other options are passed into the constructor by the dependency injection system.

### 3. The Todo Entity: `Todo/Todo.cs` (defines `TodoItem` class)

This file defines the `TodoItem` class, which represents a single todo item in our application and database.

**File Path:** `backend/API/Todo/Todo.cs`

```csharp
using System.ComponentModel.DataAnnotations; // For attributes like [Required]

namespace Todos.Todo; // Namespace for this class

public class TodoItem
{
    // Properties of a TodoItem
    public Guid Id { get; set; } // Unique identifier (Globally Unique ID)

    [Required] // Data annotation: Title is mandatory
    public string Title { get; set; } = default!; // 'default!' silences nullable warnings for required properties

    public string? Description { get; set; } // 'string?' means Description can be null (optional)

    public bool IsComplete { get; set; }

    public Priority Priority { get; set; } // Uses the Priority enum
}
```

*   **`public class TodoItem`**: A standard C# class definition.
*   **Properties (`public Guid Id { get; set; }`)**: These are like fields but with "getters" and "setters" that control access. EF Core maps these properties to columns in the database table.
*   **`Guid`**: A type for globally unique identifiers. Good for primary keys.
*   **`[Required]`**: An "attribute" that tells ASP.NET Core and EF Core that the `Title` property must have a value.
*   **`string? Description`**: The `?` indicates a "nullable reference type," meaning `Description` can hold a string or be `null`.
*   **`default!`**: This is a C# feature to tell the compiler "I know this looks like it could be null, but it will be initialized properly." It's often used with required properties that are set by EF Core or via a constructor.

### 4. Todo Priority Enum: `Todo/Priority.cs`

An "enum" (enumeration) is a type that defines a set of named constants.

**File Path:** `backend/API/Todo/Priority.cs`

```csharp
namespace Todos.Todo;

public enum Priority
{
    Lowest = 0, // Assigns 'Lowest' the value 0
    Low,        // Gets value 1 automatically
    Normal,     // Gets value 2
    High,       // Gets value 3
    Highest,    // Gets value 4
}
```
This defines different priority levels for a `TodoItem`. In C#, enums are backed by integers by default.

### 5. Data Access Logic: The Repository Pattern (`Todo/ITodoRepository.cs`)

The repository pattern is a design pattern that separates the logic for retrieving data from the rest of the application.
This file defines *both* an interface (`ITodoRepository`) and its implementation (`EntityFrameworkTodoRepository`).

**File Path:** `backend/API/Todo/ITodoRepository.cs`

**Interface (`ITodoRepository`):**
An interface defines a "contract." It specifies what methods a class *must* implement, but not *how* they are implemented.

```csharp
namespace Todos.Todo;

// ... (TodoItemSpecification abstract class is also in this file, see below) ...

public interface ITodoRepository
{
    // Adds a new TodoItem. 'async Task<TodoItem>' means it's an asynchronous
    // operation that will eventually return a TodoItem.
    Task<TodoItem> AddAsync(TodoItem item, CancellationToken cancellationToken);

    // Lists TodoItems. 'IAsyncEnumerable<TodoItem>' means it returns a stream
    // of TodoItems that can be consumed asynchronously.
    // 'params TodoItemSpecification[] specifications' means it can take zero or more specifications.
    IAsyncEnumerable<TodoItem> ListAsync(params TodoItemSpecification[] specifications);

    Task<TodoItem?> GetAsync(Guid id, CancellationToken cancellationToken); // TodoItem? means it can return null

    Task UpdateAsync(TodoItem item, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
```

*   **`async Task<T>` / `async Task`**: These indicate asynchronous methods. In C#, `async` and `await` are used for non-blocking operations, crucial for I/O-bound tasks like database calls. `Task<T>` represents an operation that will eventually return a value of type `T`. `Task` represents an operation that doesn't return a value.
*   **`CancellationToken`**: Used to signal that an operation should be cancelled. Good practice for long-running async operations.

**Implementation (`EntityFrameworkTodoRepository`):**
This class provides the actual logic for the methods defined in `ITodoRepository`, using Entity Framework Core.

```csharp
// Constructor: Takes AppDbContext via dependency injection
public class EntityFrameworkTodoRepository(AppDbContext db) : ITodoRepository
{
    public async Task<TodoItem> AddAsync(TodoItem item, CancellationToken cancellationToken)
    {
        item.Id = Guid.NewGuid(); // Generate a new ID
        db.Todos.Add(item);       // Tell EF Core to track this new item
        await db.SaveChangesAsync(cancellationToken); // Save changes to the database
        return item;
    }

    public IAsyncEnumerable<TodoItem> ListAsync(params TodoItemSpecification[] specifications)
    {
        IQueryable<TodoItem> items = db.Todos; // Start with all todos

        // Apply each specification (filter)
        foreach (var specification in specifications)
        {
            // 'specification.Expression' is a LINQ expression like 'todo => todo.Priority == somePriority'
            items = items.Where(specification.Expression);
        }
        return items.AsAsyncEnumerable(); // Return as an async stream
    }

    public async Task<TodoItem?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        // FindAsync is efficient for finding by primary key
        var item = await db.Todos.FindAsync([id], cancellationToken);
        return item;
    }

    public async Task UpdateAsync(TodoItem item, CancellationToken cancellationToken)
    {
        // ExecuteUpdateAsync is an EF Core 7+ feature for efficient updates
        var updated = await db.Todos.Where(t => t.Id == item.Id)
            .ExecuteUpdateAsync(updates => updates
                    .SetProperty(t => t.IsComplete, item.IsComplete)
                    .SetProperty(t => t.Priority, item.Priority)
                    .SetProperty(t => t.Title, item.Title)
                    .SetProperty(t => t.Description, item.Description),
                cancellationToken);

        if (updated == 0) // If no rows were affected, the item wasn't found
        {
            throw new ArgumentException("Item not found");
        }
    }

    // DeleteAsync uses ExecuteDeleteAsync, similar to UpdateAsync
    // ... (implementation is similar to UpdateAsync but calls ExecuteDeleteAsync)
}
```

*   **Constructor Injection (`EntityFrameworkTodoRepository(AppDbContext db)`)**: The `AppDbContext` is "injected" into this repository by the DI system.
*   **`await db.SaveChangesAsync()`**: This is the EF Core command that actually sends changes (inserts, updates, deletes) to the database. It's asynchronous.
*   **LINQ (`items.Where(...)`)**: Language Integrated Query (LINQ) allows you to write database queries (and query collections) using C#-like syntax. `items.Where(specification.Expression)` filters the `TodoItem`s.
*   **Lambda Expressions (`t => t.Id == item.Id`)**: A concise way to write anonymous functions. `t => t.Id == item.Id` means "for a todo item `t`, check if its `Id` property is equal to `item.Id`."

### 6. Specification Pattern (`Todo/ITodoRepository.cs` and `Todo/Specifications/`)

The Specification pattern is used here to create reusable query filters.

**Abstract Base Class (in `ITodoRepository.cs`):**
```csharp
public abstract class TodoItemSpecification
{
    // This property will hold the actual LINQ filter expression
    public abstract Expression<Func<TodoItem, bool>> Expression { get; }
}
```
*   **`abstract class`**: Cannot be instantiated directly. Meant to be a base for other classes.
*   **`Expression<Func<TodoItem, bool>>`**: A complex type representing a LINQ expression tree. It's like a blueprint for a function that takes a `TodoItem` and returns `true` or `false` (a filter condition).

**Concrete Specification (`Todo/Specifications/PriorityTodoItemSpecification.cs`):**
```csharp
namespace Todos.Todo.Specifications;

// Inherits from TodoItemSpecification
public class PriorityTodoItemSpecification(Priority priority) : TodoItemSpecification
{
    public Priority Priority { get; } = priority; // Stores the priority to filter by

    // Overrides the abstract Expression property to provide a concrete filter
    public override Expression<Func<TodoItem, bool>> Expression =>
        item => item.Priority == Priority; // Lambda expression: filter by this.Priority
}
```
This specific class creates a filter condition to find `TodoItem`s that match a given `Priority`.

### 7. API Endpoints: `Todo/Endpoints.cs`

This file uses ASP.NET Core Minimal APIs to define the HTTP endpoints (URLs) for interacting with Todos.

**File Path:** `backend/API/Todo/Endpoints.cs`
```csharp
namespace Todos.Todo;

public static class Endpoints
{
    // This is an "extension method" for IEndpointRouteBuilder
    // It groups all todo routes under "/todos"
    public static IEndpointRouteBuilder AddTodoEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("todos") // All routes here start with /todos
            .WithTags("Todos")        // For Swagger documentation
            .WithOpenApi();

        // GET /todos - lists all todos (with optional priority filter)
        group.MapGet("/", List)
            .WithName("ListTodos"); // Name for linking, etc.

        // POST /todos - creates a new todo
        group.MapPost("/", Create)
            .WithName("CreateTodo");

        // GET /todos/{id} - gets a specific todo
        group.MapGet("/{id:guid:required}", Get)
            .WithName("GetTodo");

        // PUT /todos/{id} - updates a todo
        group.MapPut("/{id:guid:required}", Save)
            .WithName("SaveTodo");

        // DELETE /todos/{id} - deletes a todo
        group.MapDelete("/{id:guid:required}", Delete)
            .WithName("DeleteTodo");

        return builder;
    }

    // Handler for GET /todos
    // ITodoRepository is injected by DI. 'Priority? priority' is an optional query parameter.
    public static IAsyncEnumerable<TodoItem> List(ITodoRepository repository, Priority? priority = null)
    {
        var specifications = new List<TodoItemSpecification>();
        if (priority.HasValue) // If priority was provided in the URL query
        {
            specifications.Add(new PriorityTodoItemSpecification(priority.Value));
        }
        return repository.ListAsync(specifications.ToArray());
    }

    // Handler for POST /todos
    // 'TodoItem item' comes from the request body (JSON)
    // 'Results<Created<TodoItem>, BadRequest>' defines possible typed results for Swagger
    public static async Task<Results<Created<TodoItem>, BadRequest>> Create(
        TodoItem item, ITodoRepository repository, CancellationToken cancellationToken)
    {
        if (item.Id != Guid.Empty) // New items shouldn't have an ID from the client
        {
            return TypedResults.BadRequest(); // Returns a 400 Bad Request
        }
        var newItem = await repository.AddAsync(item, cancellationToken);
        // Returns a 201 Created with the location and the new item
        return TypedResults.Created($"/todos/{newItem.Id}", newItem);
    }

    // ... (Get, Save, Delete methods follow a similar pattern, calling repository methods
    //      and returning TypedResults like Ok, NotFound, BadRequest) ...
}
```

*   **`public static class Endpoints`**: A static class; its methods can be called without creating an instance.
*   **Extension Method (`public static IEndpointRouteBuilder AddTodoEndpoints(this IEndpointRouteBuilder builder)`)**: The `this` keyword before the first parameter makes this an extension method. It means you can call it as if it were a method of `IEndpointRouteBuilder` itself (e.g., `app.AddTodoEndpoints()`).
*   **`MapGet`, `MapPost`, etc.**: Methods that define how HTTP GET, POST, etc. requests to specific URL patterns are handled.
*   **Route Parameters (`{id:guid:required}`):** Defines a parameter `id` in the URL. `:guid` constrains it to be a GUID, and `:required` makes it mandatory.
*   **Handler Methods (`List`, `Create`, etc.):** These static methods are executed when their corresponding endpoint is matched. Dependencies like `ITodoRepository` are automatically provided by DI.
*   **`TypedResults` (e.g., `TypedResults.Ok(item)`, `TypedResults.NotFound()`):** Provides strongly-typed results, which improves code clarity and helps Swagger/OpenAPI generate accurate API documentation.

## Frontend (React)

The frontend is in `client/` and is a standard React application.
*   **`client/src/App.js`**: Main application component.
*   **`client/src/pages/DashboardPage.js`**: Handles displaying the list of todos and the form to add new ones. It fetches data from and sends data to the backend API.
*   **`client/src/services/api.js`**: Contains JavaScript functions (using the `axios` library) to make HTTP requests to the C# backend API endpoints (e.g., GET `/todos`, POST `/todos`).
*   **`client/src/components/`**: Contains reusable UI pieces like `TodoList.js`, `TodoCard.js`, and `AddTodo.js`.

The `REACT_APP_API_URL` environment variable in the client is used to tell the React app where the backend API is running. This is set by .NET Aspire when running that way.

## .NET Aspire (`tools/AppHost/`)

.NET Aspire is a tool for building and running distributed applications (applications made of multiple services). The `tools/AppHost/Program.cs` file defines how the different parts of this application (frontend, backend API, database) are connected and run.

**Key Snippet from `tools/AppHost/Program.cs`:**
```csharp
// Defines the PostgreSQL database container
var db = builder.AddPostgres("db", ...)
    .AddDatabase("database", databaseName: "todo");

// Defines the backend API project, tells it to use the 'db' resource
var api = builder.AddProject<Projects.API>("api")
    .WithReference(db);

// Defines the frontend NPM app, tells it the API's URL
var web = builder.AddNpmApp("web", "../../client")
    .WithReference(api)
    .WithEnvironment("REACT_APP_API_URL", api.GetEndpoint("http"));

builder.Build().Run();
```
This C# code uses Aspire's "builder" API to:
1.  Define a PostgreSQL database container.
2.  Define the backend API project and automatically configure its connection string to point to the PostgreSQL container.
3.  Define the frontend React (NPM) app and set an environment variable (`REACT_APP_API_URL`) so the frontend knows how to reach the backend.

When you run `dotnet run --project ./tools/AppHost`, Aspire starts all these components.

## Key Discussion Points for an Interview

Given the recruiter's statement ("facilitate an interview and speaking about different code structure choices"), here are things to consider:

1.  **Missing Edit Functionality:** The application can create, list, and delete todos, but not edit existing ones.
    *   *How would you add this?* (Backend: New PUT endpoint in `Endpoints.cs`, `UpdateAsync` method in `ITodoRepository` already exists. Frontend: Update UI in `AddTodo.js` or a new component, new function in `api.js`, state management in `DashboardPage.js`).
2.  **`IsComplete` Status:**
    *   The `TodoItem` has an `IsComplete` property.
    *   The `AddTodo.js` form defaults it to `true` for new todos (unusual) and there's no UI to change it.
    *   `TodoCard.js` doesn't display it.
    *   *How would you integrate this properly?*
3.  **`addingTodo` Prop (Frontend):** The `addingTodo` state in `DashboardPage.js` and its use as a `todo` prop in `AddTodo.js` is a bit confusing if `AddTodo` is only for new items. It seems partially set up for editing but isn't used that way.
4.  **Error Handling:** Basic in both frontend (`console.error`) and backend (throwing `ArgumentException` which Minimal APIs turn into `404 Not Found` or `400 Bad Request`). Could be more user-friendly.
5.  **Efficiency (Frontend):** After creating or deleting a todo, the entire list is re-fetched. For larger lists, "optimistic updates" (updating the UI immediately and then syncing with the backend) could be better.
6.  **Backend Design Choices:**
    *   **Repository & Specification Patterns:** Why use them? (Decoupling, testability, reusable query logic).
    *   **Minimal APIs vs. Controllers:** Pros and cons. (Minimal APIs are more concise for simple APIs; controllers offer more structure for complex ones).
    *   **EF Core `ExecuteUpdateAsync` / `ExecuteDeleteAsync`:** Benefits? (More efficient as they don't load entities into memory first).
7.  **.NET Aspire:** What problems does it solve for multi-service applications?
8.  **`Question.js` Component (Frontend):** This component isn't used in the main app flow but seems designed to display toggleable questions/info – likely a meta-hint that the interview might involve such prompts.

## Testing

The `README.md` mentions several testing tools and projects:
*   **Backend Testing:**
    *   `API.Tests` (Unit Tests): Uses NUnit (test framework), FakeItEasy (mocking), AutoFixture (test data generation).
    *   `API.Integration.Tests`: Uses WebApplicationFactory (for in-memory testing of the API), TestContainers (for running real dependencies like PostgreSQL in Docker for tests), Respawn (for resetting databases between tests).
*   **End-to-End Testing:**
    *   `e2e/EndToEnd.Tests`: Uses Playwright (for browser automation testing).

This shows a commitment to different levels of testing, which is good practice.

---

This explanation should give you a better grasp of the C# parts of the codebase. Focus on understanding the roles of `Program.cs`, `AppDbContext.cs`, the `TodoItem` entity, the repository, and the `Endpoints.cs` file, as these are central to the backend's operation. Good luck with your interview! 