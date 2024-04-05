using System.Diagnostics.CodeAnalysis;

namespace Todos.Todo;

public static class Endpoints
{
    [ExcludeFromCodeCoverage]
    public static IEndpointRouteBuilder AddTodoEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("todos")
            .WithTags("Todos")
            .WithOpenApi();

        group.MapGet("/", List)
            .WithName("ListTodos");

        group.MapPost("/", Create)
            .WithName("CreateTodo");

        group.MapGet("/{id:guid:required}", Get)
            .WithName("GetTodo");

        group.MapPut("/{id:guid:required}", Save)
            .WithName("SaveTodo");

        group.MapDelete("/{id:guid:required}", Delete)
            .WithName("DeleteTodo");

        return builder;
    }

    public static IAsyncEnumerable<TodoItem> List(ITodoRepository repository) => repository.ListAsync();

    public static async Task<Results<Created<TodoItem>, BadRequest>> Create(TodoItem item, ITodoRepository repository, CancellationToken cancellationToken)
    {
        if (item.Id != Guid.Empty)
        {
            return BadRequest();
        }

        var newItem = await repository.AddAsync(item, cancellationToken);

        return Created($"/todos/{newItem.Id}", newItem);
    }

    public static async Task<Results<Ok<TodoItem>, NotFound>> Get(Guid id, ITodoRepository repository, CancellationToken cancellationToken)
    {
        var item = await repository.GetAsync(id, cancellationToken);

        if (item is null)
        {
            return NotFound();
        }

        return Ok(item);
    }

    public static async Task<Results<Ok<TodoItem>, BadRequest, NotFound>> Save(Guid id, TodoItem item, ITodoRepository repository, CancellationToken cancellationToken)
    {
        if (item.Id != Guid.Empty && item.Id != id)
        {
            return BadRequest();
        }

        try
        {
            await repository.UpdateAsync(item, cancellationToken);
        }
        catch (ArgumentException)
        {
            return NotFound();
        }

        return Ok(item);
    }

    public static async Task<Results<Ok, NotFound>> Delete(Guid id, ITodoRepository repository, CancellationToken cancellationToken)
    {
        try
        {
            await repository.DeleteAsync(id, cancellationToken);
        }
        catch (ArgumentException)
        {
            return NotFound();
        }

        return Ok();
    }
}