using BlazorTodo.Models;
using BlazorTodo.Repositories;

namespace BlazorTodo.Services;

/// <summary>
/// Implementation of ITodoService with business logic and validation.
/// </summary>
public class TodoService : ITodoService
{
    private readonly ITodoRepository _repository;
    private readonly ILogger<TodoService> _logger;

    public TodoService(ITodoRepository repository, ILogger<TodoService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TodoItemDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetAllAsync(cancellationToken);
        return items.Select(TodoItemDto.FromTodoItem);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TodoItemDto>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetAllAsync(cancellationToken);
        return items.Where(x => !x.IsCompleted).Select(TodoItemDto.FromTodoItem);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TodoItemDto>> GetCompletedAsync(CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetAllAsync(cancellationToken);
        return items.Where(x => x.IsCompleted).Select(TodoItemDto.FromTodoItem);
    }

    /// <inheritdoc/>
    public async Task<TodoItemDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _repository.GetByIdAsync(id, cancellationToken);
        return item is not null ? TodoItemDto.FromTodoItem(item) : null;
    }

    /// <inheritdoc/>
    public async Task<TodoItemDto> CreateAsync(TodoItemDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (string.IsNullOrWhiteSpace(dto.Title))
        {
            throw new ArgumentException("Title is required.", nameof(dto));
        }

        _logger.LogInformation("Creating new todo item: {Title}", dto.Title);

        var item = TodoItemFactory.Create(
            dto.Title,
            dto.Description,
            dto.DueDate,
            dto.Priority);

        var addedItem = await _repository.AddAsync(item, cancellationToken);
        return TodoItemDto.FromTodoItem(addedItem);
    }

    /// <inheritdoc/>
    public async Task<TodoItemDto?> UpdateAsync(TodoItemDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (string.IsNullOrWhiteSpace(dto.Title))
        {
            throw new ArgumentException("Title is required.", nameof(dto));
        }

        var existingItem = await _repository.GetByIdAsync(dto.Id, cancellationToken);
        if (existingItem is null)
        {
            _logger.LogWarning("Attempted to update non-existent todo item: {Id}", dto.Id);
            return null;
        }

        _logger.LogInformation("Updating todo item: {Id} - {Title}", dto.Id, dto.Title);

        var updatedItem = TodoItemFactory.Update(
            existingItem,
            dto.Title,
            dto.Description,
            dto.DueDate,
            dto.Priority);

        // Preserve completion state from DTO
        if (dto.IsCompleted != updatedItem.IsCompleted)
        {
            updatedItem = updatedItem with
            {
                IsCompleted = dto.IsCompleted,
                CompletedAt = dto.IsCompleted ? (dto.CompletedAt ?? DateTime.UtcNow) : null
            };
        }

        var result = await _repository.UpdateAsync(updatedItem, cancellationToken);
        return result is not null ? TodoItemDto.FromTodoItem(result) : null;
    }

    /// <inheritdoc/>
    public async Task<TodoItemDto?> ToggleCompleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var existingItem = await _repository.GetByIdAsync(id, cancellationToken);
        if (existingItem is null)
        {
            _logger.LogWarning("Attempted to toggle completion on non-existent todo item: {Id}", id);
            return null;
        }

        var newCompletionState = !existingItem.IsCompleted;
        _logger.LogInformation("Toggling completion for todo item: {Id} - New state: {State}", id, newCompletionState);

        var updatedItem = existingItem with
        {
            IsCompleted = newCompletionState,
            CompletedAt = newCompletionState ? DateTime.UtcNow : null
        };

        var result = await _repository.UpdateAsync(updatedItem, cancellationToken);
        return result is not null ? TodoItemDto.FromTodoItem(result) : null;
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting todo item: {Id}", id);
        return await _repository.DeleteAsync(id, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<PagedResult<TodoItemDto>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting paged todo items: Page {PageNumber}, Size {PageSize}", pageNumber, pageSize);
        var pagedResult = await _repository.GetPagedAsync(pageNumber, pageSize, cancellationToken);
        
        var dtos = pagedResult.Items.Select(item => new TodoItemDto
        {
            Id = item.Id,
            Title = item.Title,
            IsCompleted = item.IsCompleted,
            Priority = item.Priority,
            CreatedAt = item.CreatedAt
        });

        return new PagedResult<TodoItemDto>
        {
            Items = dtos,
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize,
            TotalCount = pagedResult.TotalCount
        };
    }
}
