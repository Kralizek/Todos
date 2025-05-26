// Imports the Microsoft.EntityFrameworkCore namespace, which contains base classes for EF Core functionality like DbContext.
using Microsoft.EntityFrameworkCore;

// Imports the Todos.Todo namespace, where the TodoItem entity is defined (in Todo/Todo.cs).
using Todos.Todo;

// Defines the namespace for this AppDbContext class. Namespaces help organize code and prevent naming conflicts.
namespace Todos;

// Declares the AppDbContext class. It inherits from DbContext, which is the primary class for interacting with a database in EF Core.
// The constructor 'AppDbContext(DbContextOptions<AppDbContext> options)' is a common pattern for DbContext classes.
// 'DbContextOptions<AppDbContext> options' are passed in by the dependency injection system (configured in Program.cs).
// These options include the database provider to use (e.g., PostgreSQL) and the connection string.
// The ' : DbContext(options)' part calls the base class (DbContext) constructor, passing along the options.
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    // This line declares a public property named 'Todos' of type DbSet<TodoItem>.
    // A DbSet represents a collection of entities (like a table in the database) that can be queried and saved.
    // 'TodoItem' is the entity class (defined in Todo/Todo.cs) that maps to rows in the 'todos' table.
    // 'Set<TodoItem>()' is an EF Core method that creates/retrieves the DbSet for the TodoItem entity.
    // Other parts of the application (like the EntityFrameworkTodoRepository) will use this 'Todos' property
    // to perform database operations (e.g., db.Todos.Add(item), db.Todos.Where(...).ToList()).
    public DbSet<TodoItem> Todos => Set<TodoItem>();

    // This method, 'OnModelCreating', is overridden from the base DbContext class.
    // EF Core calls this method when it is building the database model for the first time.
    // It allows you to configure how your entities map to the database schema (e.g., table names, column types, relationships, constraints).
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 'modelBuilder.Entity<TodoItem>()' gets a builder object for configuring the TodoItem entity.
        // '.ToTable("todos")' explicitly maps the TodoItem entity to a database table named "todos".
        // If you didn't specify this, EF Core might use a default name like "TodoItems".
        var todo = modelBuilder.Entity<TodoItem>().ToTable("todos");

        // Configures the 'Title' property of the TodoItem entity.
        // 'todo.Property(p => p.Title)' selects the Title property (using a lambda expression).
        // '.HasMaxLength(128)' specifies that the corresponding column in the database should have a maximum length of 128 characters.
        // This helps enforce data integrity at the database level.
        todo.Property(p => p.Title).HasMaxLength(128);

        // Calls the base class's OnModelCreating method. It's good practice to do this
        // in case the base class has its own model configuration logic.
        base.OnModelCreating(modelBuilder);
    }
}