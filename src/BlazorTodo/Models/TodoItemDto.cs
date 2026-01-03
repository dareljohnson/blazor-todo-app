namespace BlazorTodo.Models;

/// <summary>
/// Data transfer object for TodoItem, used for component binding and API communication.
/// </summary>
public record TodoItemDto
{
    /// <summary>
    /// Unique identifier (Guid.Empty for new items).
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Title of the todo item.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Optional description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Indicates if completed.
    /// </summary>
    public bool IsCompleted { get; set; }

    /// <summary>
    /// Optional due date.
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Priority level.
    /// </summary>
    public TodoPriority Priority { get; set; } = TodoPriority.Low;

    /// <summary>
    /// Creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Completion timestamp (null if not completed).
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Creates a DTO from a TodoItem domain model.
    /// </summary>
    public static TodoItemDto FromTodoItem(TodoItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return new TodoItemDto
        {
            Id = item.Id,
            Title = item.Title,
            Description = item.Description,
            IsCompleted = item.IsCompleted,
            DueDate = item.DueDate,
            Priority = item.Priority,
            CreatedAt = item.CreatedAt,
            CompletedAt = item.CompletedAt
        };
    }

    /// <summary>
    /// Converts this DTO to a TodoItem domain model.
    /// </summary>
    public TodoItem ToTodoItem()
    {
        return new TodoItem
        {
            Id = Id,
            Title = Title,
            Description = Description,
            IsCompleted = IsCompleted,
            DueDate = DueDate,
            Priority = Priority,
            CreatedAt = CreatedAt,
            CompletedAt = CompletedAt
        };
    }
}
