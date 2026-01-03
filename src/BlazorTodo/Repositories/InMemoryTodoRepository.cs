using System.Collections.Concurrent;
using BlazorTodo.Models;

namespace BlazorTodo.Repositories;

/// <summary>
/// Thread-safe in-memory implementation of ITodoRepository.
/// </summary>
public class InMemoryTodoRepository : ITodoRepository
{
    private readonly ConcurrentDictionary<Guid, TodoItem> _items = new();

    /// <inheritdoc/>
    public Task<IEnumerable<TodoItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_items.Values.OrderByDescending(x => x.CreatedAt).AsEnumerable());
    }

    /// <inheritdoc/>
    public Task<TodoItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _items.TryGetValue(id, out var item);
        return Task.FromResult(item);
    }

    /// <inheritdoc/>
    public Task<TodoItem> AddAsync(TodoItem item, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(item);
        cancellationToken.ThrowIfCancellationRequested();

        if (!_items.TryAdd(item.Id, item))
        {
            throw new InvalidOperationException($"Item with ID {item.Id} already exists.");
        }

        return Task.FromResult(item);
    }

    /// <inheritdoc/>
    public Task<TodoItem?> UpdateAsync(TodoItem item, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(item);
        cancellationToken.ThrowIfCancellationRequested();

        if (_items.TryGetValue(item.Id, out var existingItem))
        {
            _items[item.Id] = item;
            return Task.FromResult<TodoItem?>(item);
        }

        return Task.FromResult<TodoItem?>(null);
    }

    /// <inheritdoc/>
    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = _items.TryRemove(id, out _);
        return Task.FromResult(result);
    }

    /// <inheritdoc/>
    public Task<PagedResult<TodoItem>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        var allItems = _items.Values.OrderByDescending(x => x.CreatedAt).ToList();
        var totalCount = allItems.Count;
        var items = allItems
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var result = new PagedResult<TodoItem>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };

        return Task.FromResult(result);
    }
}
