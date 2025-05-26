// Defines the namespace for this Priority enum.
// This means it belongs to the same logical group as TodoItem, ITodoRepository, etc.
// Other files can use \'using Todos.Todo;\' to access this enum.
namespace Todos.Todo;

// Declares a public enum (enumeration) named \'Priority\'.
// An enum is a special value type that defines a set of named constants.
// This is used by the TodoItem class (in TodoItem.Priority) to represent the
// importance level of a todo task in a type-safe and readable way.
public enum Priority
{
    // Defines the named constants (members) of the Priority enum.
    // By default, enum members are assigned underlying integer values starting from 0.

    Lowest = 0, // Explicitly assigns the integer value 0 to \'Lowest\'.
    Low,        // Implicitly assigned the next integer value, which is 1.
    Normal,     // Implicitly assigned the next integer value, which is 2.
    High,       // Implicitly assigned the next integer value, which is 3.
    Highest,    // Implicitly assigned the next integer value, which is 4.

    // Using this enum, code can refer to priorities like \'Priority.High\'
    // instead of magic numbers (e.g., 3), making the code clearer and less error-prone.
    // This enum will be stored as its integer value in the database for the TodoItem.Priority field.
}