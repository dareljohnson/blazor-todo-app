using BlazorTodo.Models;

namespace BlazorTodo.Repositories;

/// <summary>
/// Repository interface for TodoItem data access operations.
/// </summary>
public interface ITodoRepository
{
    /// <summary>
    /// Gets all todo items.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of all todo items.</returns>
    Task<IEnumerable<TodoItem>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a todo item by its ID.
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The todo item if found; otherwise null.</returns>
    Task<TodoItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new todo item.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The added item.</returns>
    Task<TodoItem> AddAsync(TodoItem item, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing todo item.
    /// </summary>
    /// <param name="item">The item to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated item if successful; otherwise null.</returns>
    Task<TodoItem?> UpdateAsync(TodoItem item, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a todo item by its ID.
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if deleted; otherwise false.</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
