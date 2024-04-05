using Microsoft.EntityFrameworkCore;

using Todos.Todo;

namespace Todos;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<TodoItem> Todos => Set<TodoItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var todo = modelBuilder.Entity<TodoItem>().ToTable("todos");

        todo.Property(p => p.Title).HasMaxLength(128);

        base.OnModelCreating(modelBuilder);
    }
}
