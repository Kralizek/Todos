namespace Todos.Todo.Specifications;

public class PriorityTodoItemSpecification(Priority priority) : TodoItemSpecification
{
    public Priority Priority { get; } = priority;
    
    public override Expression<Func<TodoItem, bool>> Expression => item => item.Priority == Priority;
}
