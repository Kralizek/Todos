using System.ComponentModel.DataAnnotations;

namespace Todos.Todo;

public class TodoItem
{
    // Public property named 'Id' of type Guid (Globally Unique Identifier).
    // '{ get; set; }' defines an auto-implemented property with a getter and a setter.
    // This will be the primary key for the TodoItem in the database.
    // Guid is often used for primary keys to ensure uniqueness across distributed systems.
    public Guid Id { get; set; }

    // The [Required] attribute is a data annotation.
    // It indicates that the 'Title' property must have a value.
    // This is used by: 
    //  1. Entity Framework Core for database schema generation (e.g., to make the column NOT NULL).
    //  2. ASP.NET Core for request model validation (if this object is received in an HTTP request).
    [Required]
    // Public property named 'Title' of type string.
    // The '= default!;' part is a C# feature related to nullable reference types.
    // It essentially tells the compiler "I know this is non-nullable and looks uninitialized,
    // but it will be assigned a value (e.g., by EF Core when fetching from DB, or by user input before saving)."
    public string Title { get; set; } = default!;

    // Public property named 'Description' of type 'string?'.
    // The '?' indicates that this is a nullable reference type, meaning 'Description' can hold a string
    // or it can be 'null', representing an optional description.
    public string? Description { get; set; }

    // Public property named 'IsComplete' of type bool (boolean - true or false).
    // Indicates whether the todo task has been completed.
    public bool IsComplete { get; set; }

    // Public property named 'Priority' of type 'Priority'.
    // 'Priority' is an enum type defined in Priority.cs (in the same Todos.Todo namespace).
    // This property will hold one of the defined priority values (e.g., Lowest, Normal, Highest).
    // Enums help create more readable and type-safe code compared to using raw numbers or strings for such states.
    public Priority Priority { get; set; }
}
