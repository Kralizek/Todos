using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http.HttpResults;

using SQLitePCL;

using Todos.Model;

namespace Todos.Todo;

public static class Endpoints
{
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

    public static IAsyncEnumerable<TodoItem> List(TodoDbContext db) => db.Todos.AsAsyncEnumerable();

    public static async Task<Results<Created<TodoItem>, BadRequest>> Create(TodoItem item, TodoDbContext db, CancellationToken cancellationToken)
    {
        if (item.Id != Guid.Empty)
        {
            return BadRequest();
        }

        item.Id = Guid.NewGuid();

        db.Todos.Add(item);

        await db.SaveChangesAsync(cancellationToken);

        return Created($"/todos/{item.Id}", item);
    }

    public static async Task<Results<Ok<TodoItem>, NotFound>> Get(Guid id, TodoDbContext db, CancellationToken cancellationToken)
    {
        var item = await db.Todos.FindAsync([id], cancellationToken);

        if (item is null)
        {
            return NotFound();
        }

        return Ok(item);
    }

    public static async Task<Results<Ok<TodoItem>, BadRequest, NotFound>> Save(Guid id, TodoItem item, TodoDbContext db, CancellationToken cancellationToken)
    {
        if (item.Id != id)
        {
            return BadRequest();
        }

        var updated = await db.Todos.Where(t => t.Id == id)
            .ExecuteUpdateAsync(updates => updates
                    .SetProperty(t => t.IsComplete, item.IsComplete)
                    .SetProperty(t => t.Priority, item.Priority)
                    .SetProperty(t => t.Title, item.Title)
                    .SetProperty(t => t.Description, item.Description),
                cancellationToken);

        return updated switch 
        {
            0 => NotFound(),
            _ => Ok(item)
        };
    }

    public static async Task<Results<Ok, NotFound>> Delete(Guid id, TodoDbContext db, CancellationToken cancellationToken)
    {
        var deleted = await db.Todos.Where(t => t.Id == id).ExecuteDeleteAsync(cancellationToken);

        return deleted switch 
        {
            0 => NotFound(),
            _ => Ok()
        };
    }
}