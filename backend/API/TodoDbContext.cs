using Microsoft.EntityFrameworkCore;

using Todos.Model;

namespace Todos;

public class TodoDbContext(DbContextOptions<TodoDbContext> options) : DbContext(options)
{
    public DbSet<TodoItem> Todos => Set<TodoItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var todo = modelBuilder.Entity<TodoItem>().ToTable("todos");

        todo.Property(p => p.Title).HasMaxLength(128);

        base.OnModelCreating(modelBuilder);
    }
}
