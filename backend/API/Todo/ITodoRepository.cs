using System.Linq.Expressions;

namespace Todos.Todo;

public abstract class TodoItemSpecification
{
    public Expression<Func<TodoItem, bool>> Expression { get; }
}

public interface ITodoRepository
{
    Task<TodoItem> AddAsync(TodoItem item, CancellationToken cancellationToken);

    IAsyncEnumerable<TodoItem> ListAsync(params TodoItemSpecification[] specifications);

    Task<TodoItem?> GetAsync(Guid id, CancellationToken cancellationToken);

    Task UpdateAsync(TodoItem item, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}

public class EntityFrameworkTodoRepository(AppDbContext db) : ITodoRepository
{
    public async Task<TodoItem> AddAsync(TodoItem item, CancellationToken cancellationToken)
    {
        item.Id = Guid.NewGuid();

        db.Todos.Add(item);

        await db.SaveChangesAsync(cancellationToken);

        return item;
    }

    public IAsyncEnumerable<TodoItem> ListAsync(params TodoItemSpecification[] specifications)
    {
        IQueryable<TodoItem> items = db.Todos;

        foreach (var specification in specifications)
        {
            items = items.Where(specification.Expression);
        }

        return items.AsAsyncEnumerable();
    }

    public async Task<TodoItem?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var item = await db.Todos.FindAsync([id], cancellationToken);

        return item;
    }

    public async Task UpdateAsync(TodoItem item, CancellationToken cancellationToken)
    {
        var updated = await db.Todos.Where(t => t.Id == item.Id)
            .ExecuteUpdateAsync(updates => updates
                    .SetProperty(t => t.IsComplete, item.IsComplete)
                    .SetProperty(t => t.Priority, item.Priority)
                    .SetProperty(t => t.Title, item.Title)
                    .SetProperty(t => t.Description, item.Description),
                cancellationToken);

        if (updated == 0)
        {
            throw new ArgumentException("Item not found");
        }
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await db.Todos.Where(t => t.Id == id).ExecuteDeleteAsync(cancellationToken);
        
        if (deleted == 0)
        {
            throw new ArgumentException("Item not found");
        }
    }
}