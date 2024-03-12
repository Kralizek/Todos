using System.ComponentModel.DataAnnotations;

namespace Todos.Model;

public class TodoItem
{
    public Guid Id { get; set; }

    [Required]
    public string Title { get; set; } = default!;

    public string? Description { get; set; }

    public bool IsComplete { get; set; }

    public Priority Priority { get; set; }
}
