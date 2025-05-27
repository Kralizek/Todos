// Namespaces are used to organize code and prevent naming conflicts.
// This namespace groups all code related to the 'Todo' feature.
namespace Todos.Todo;

// An abstract class is a class that cannot be instantiated directly.
// It's meant to be inherited by other classes.
// This class serves as a base for creating different specifications (filters) for TodoItems.
public abstract class TodoItemSpecification
{
    // This is an abstract property, meaning that any class inheriting from TodoItemSpecification
    // must provide an implementation for it.
    // It defines an expression that can be used to filter TodoItem objects.
    // Expression<Func<TodoItem, bool>> is a type that represents a lambda expression
    // which takes a TodoItem as input and returns a boolean (true if the item matches the filter, false otherwise).
    public abstract Expression<Func<TodoItem, bool>> Expression { get; }
}

// An interface defines a contract for classes.
// Any class implementing ITodoRepository must provide implementations for the methods defined here.
// This interface outlines the operations that can be performed on TodoItems.
public interface ITodoRepository
{
    // Task<TodoItem> indicates that this method is asynchronous and will eventually return a TodoItem.
    // 'async' and 'await' keywords are typically used with Task to handle asynchronous operations.
    // This method adds a new TodoItem to the repository.
    // CancellationToken is used to signal if the operation should be cancelled.
    Task<TodoItem> AddAsync(TodoItem item, CancellationToken cancellationToken);

    // IAsyncEnumerable<TodoItem> indicates that this method returns a sequence of TodoItems that can be iterated asynchronously.
    // This method lists TodoItems, optionally filtering them based on the provided specifications.
    // 'params' allows passing a variable number of TodoItemSpecification arguments.
    IAsyncEnumerable<TodoItem> ListAsync(params TodoItemSpecification[] specifications);

    // This method retrieves a specific TodoItem by its ID.
    // It returns a Task<TodoItem?>, where '?' indicates that the TodoItem can be null (if not found).
    Task<TodoItem?> GetAsync(Guid id, CancellationToken cancellationToken);

    // This method updates an existing TodoItem.
    Task UpdateAsync(TodoItem item, CancellationToken cancellationToken);

    // This method deletes a TodoItem by its ID.
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}

// This class provides an implementation of the ITodoRepository interface using Entity Framework Core.
// Entity Framework Core is an Object-Relational Mapper (ORM) that simplifies database interactions.
// The '(AppDbContext db)' is a primary constructor, injecting an AppDbContext instance.
// AppDbContext likely represents the database session.
public class EntityFrameworkTodoRepository(AppDbContext db) : ITodoRepository
{
    // Implements the AddAsync method from the ITodoRepository interface.
    public async Task<TodoItem> AddAsync(TodoItem item, CancellationToken cancellationToken)
    {
        // Generates a new unique identifier (GUID) for the TodoItem.
        item.Id = Guid.NewGuid();

        // Adds the new TodoItem to the DbSet<TodoItem> (likely named 'Todos') in the AppDbContext.
        // This stages the item for insertion into the database.
        db.Todos.Add(item);

        // Asynchronously saves all changes made in this context to the database.
        await db.SaveChangesAsync(cancellationToken);

        // Returns the added TodoItem (which now includes the generated ID).
        return item;
    }

    // Implements the ListAsync method from the ITodoRepository interface.
    public IAsyncEnumerable<TodoItem> ListAsync(params TodoItemSpecification[] specifications)
    {
        // Starts with an IQueryable<TodoItem> representing all TodoItems in the database.
        // IQueryable allows building up database queries dynamically.
        IQueryable<TodoItem> items = db.Todos;

        // Iterates through each provided specification (filter).
        foreach (var specification in specifications)
        {
            // Applies the filter defined by the specification's Expression to the query.
            // The 'Where' method is a LINQ (Language Integrated Query) extension method.
            items = items.Where(specification.Expression);
        }

        // Converts the IQueryable to an IAsyncEnumerable, allowing asynchronous iteration over the results.
        return items.AsAsyncEnumerable();
    }

    // Implements the GetAsync method from the ITodoRepository interface.
    public async Task<TodoItem?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        // Asynchronously finds a TodoItem by its primary key (ID).
        // The 'FindAsync' method is an efficient way to retrieve an entity by its key.
        var item = await db.Todos.FindAsync([id], cancellationToken);

        // Returns the found item, or null if no item with the given ID exists.
        return item;
    }

    // Implements the UpdateAsync method from the ITodoRepository interface.
    public async Task UpdateAsync(TodoItem item, CancellationToken cancellationToken)
    {
        // Constructs an update query for the TodoItem with the matching ID.
        // 'Where(t => t.Id == item.Id)' filters for the specific item.
        // 'ExecuteUpdateAsync' executes the update directly in the database without loading the entity.
        var updated = await db.Todos.Where(t => t.Id == item.Id)
            .ExecuteUpdateAsync(updates => updates
                    // Specifies which properties to update and their new values.
                    .SetProperty(t => t.IsComplete, item.IsComplete)
                    .SetProperty(t => t.Priority, item.Priority)
                    .SetProperty(t => t.Title, item.Title)
                    .SetProperty(t => t.Description, item.Description),
                cancellationToken);

        // 'ExecuteUpdateAsync' returns the number of rows affected.
        // If no rows were affected, it means the item was not found.
        if (updated == 0)
        {
            // Throws an ArgumentException to indicate that the item to be updated was not found.
            throw new ArgumentException("Item not found");
        }
    }

    // Implements the DeleteAsync method from the ITodoRepository interface.
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        // Constructs a delete query for the TodoItem with the matching ID.
        // 'ExecuteDeleteAsync' executes the delete operation directly in the database.
        var deleted = await db.Todos.Where(t => t.Id == id).ExecuteDeleteAsync(cancellationToken);
        
        // 'ExecuteDeleteAsync' returns the number of rows affected.
        // If no rows were affected, it means the item was not found.
        if (deleted == 0)
        {
            // Throws an ArgumentException to indicate that the item to be deleted was not found.
            throw new ArgumentException("Item not found");
        }
    }
}