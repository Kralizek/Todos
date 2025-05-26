namespace Todos.Todo.Specifications;

// Declares the PriorityTodoItemSpecification class.
// It inherits from the abstract TodoItemSpecification class (defined in ITodoRepository.cs).
// This means it must provide a concrete implementation for the abstract 'Expression' property.
// The constructor 'PriorityTodoItemSpecification(Priority priority)' takes a Priority enum value.
// This 'priority' value will be used to build the specific filter expression.
public class PriorityTodoItemSpecification(Priority priority) : TodoItemSpecification
{
    // Public read-only property to store the priority value passed to the constructor.
    // 'get;' means it has a getter, but no public setter, so it can only be set during object initialization (e.g., in the constructor).
    // The 'priority' parameter from the constructor is assigned to this property.
    public Priority Priority { get; } = priority;
    
    // Overrides the abstract 'Expression' property inherited from TodoItemSpecification.
    // 'override' keyword is used to provide a specific implementation for an abstract or virtual member of a base class.
    // '=> item => item.Priority == Priority;' is a lambda expression defining the filter logic.
    //   - 'item' represents an instance of TodoItem (the input to the Func<TodoItem, bool>).
    //   - 'item.Priority == Priority' is the boolean condition. It checks if the 'Priority' property of the 'item'
    //     is equal to the 'Priority' property of this PriorityTodoItemSpecification instance (which was set in the constructor).
    // This expression will be used by Entity Framework Core (via the repository's ListAsync method)
    // to translate into a SQL WHERE clause (e.g., WHERE priority = <value_of_this.Priority>).
    public override Expression<Func<TodoItem, bool>> Expression => item => item.Priority == Priority;
}
