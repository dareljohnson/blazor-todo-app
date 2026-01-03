namespace BlazorTodo.Models;

/// <summary>
/// Represents an immutable todo item with all its properties.
/// </summary>
public record TodoItem
{
    /// <summary>
    /// Unique identifier for the todo item.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Title of the todo item (required, non-empty).
    /// </summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Optional detailed description of the todo item.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Indicates whether the todo item has been completed.
    /// </summary>
    public bool IsCompleted { get; init; }

    /// <summary>
    /// Optional due date for the todo item.
    /// </summary>
    public DateTime? DueDate { get; init; }

    /// <summary>
    /// Priority level of the todo item.
    /// </summary>
    public TodoPriority Priority { get; init; }

    /// <summary>
    /// Timestamp when the todo item was created.
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Timestamp when the todo item was completed (null if not completed).
    /// </summary>
    public DateTime? CompletedAt { get; init; }
}

/// <summary>
/// Factory for creating validated TodoItem instances.
/// </summary>
public static class TodoItemFactory
{
    /// <summary>
    /// Creates a new TodoItem with validation.
    /// </summary>
    /// <param name="title">The title of the todo item (required).</param>
    /// <param name="description">Optional description.</param>
    /// <param name="dueDate">Optional due date.</param>
    /// <param name="priority">Priority level (default: Low).</param>
    /// <returns>A new validated TodoItem instance.</returns>
    /// <exception cref="ArgumentException">Thrown when title is null, empty, or whitespace.</exception>
    public static TodoItem Create(
        string title,
        string? description = null,
        DateTime? dueDate = null,
        TodoPriority priority = TodoPriority.Low)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title cannot be empty or whitespace.", nameof(title));
        }

        return new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = title.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            IsCompleted = false,
            DueDate = dueDate,
            Priority = priority,
            CreatedAt = DateTime.UtcNow,
            CompletedAt = null
        };
    }

    /// <summary>
    /// Creates a TodoItem from an existing item (for updates via 'with' expressions).
    /// </summary>
    /// <param name="existingItem">The existing item to base the new item on.</param>
    /// <param name="title">Optional new title.</param>
    /// <param name="description">Optional new description.</param>
    /// <param name="dueDate">Optional new due date.</param>
    /// <param name="priority">Optional new priority.</param>
    /// <returns>A new TodoItem with updated properties.</returns>
    public static TodoItem Update(
        TodoItem existingItem,
        string? title = null,
        string? description = null,
        DateTime? dueDate = null,
        TodoPriority? priority = null)
    {
        ArgumentNullException.ThrowIfNull(existingItem);

        var newTitle = title ?? existingItem.Title;
        if (string.IsNullOrWhiteSpace(newTitle))
        {
            throw new ArgumentException("Title cannot be empty or whitespace.", nameof(title));
        }

        return existingItem with
        {
            Title = newTitle.Trim(),
            Description = description is not null 
                ? (string.IsNullOrWhiteSpace(description) ? null : description.Trim())
                : existingItem.Description,
            DueDate = dueDate ?? existingItem.DueDate,
            Priority = priority ?? existingItem.Priority
        };
    }
}
