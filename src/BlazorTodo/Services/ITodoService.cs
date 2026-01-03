using BlazorTodo.Models;

namespace BlazorTodo.Services;

/// <summary>
/// Service interface for todo business logic operations.
/// </summary>
public interface ITodoService
{
    /// <summary>
    /// Gets all todo items.
    /// </summary>
    Task<IEnumerable<TodoItemDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets only active (incomplete) todo items.
    /// </summary>
    Task<IEnumerable<TodoItemDto>> GetActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets only completed todo items.
    /// </summary>
    Task<IEnumerable<TodoItemDto>> GetCompletedAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a todo item by its ID.
    /// </summary>
    Task<TodoItemDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new todo item.
    /// </summary>
    Task<TodoItemDto> CreateAsync(TodoItemDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing todo item.
    /// </summary>
    Task<TodoItemDto?> UpdateAsync(TodoItemDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Toggles the completion status of a todo item.
    /// </summary>
    Task<TodoItemDto?> ToggleCompleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a todo item.
    /// </summary>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
