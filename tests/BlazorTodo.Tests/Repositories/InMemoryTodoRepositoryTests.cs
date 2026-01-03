using BlazorTodo.Models;
using BlazorTodo.Repositories;
using FluentAssertions;
using Xunit;

namespace BlazorTodo.Tests.Repositories;

public class InMemoryTodoRepositoryTests
{
    private readonly InMemoryTodoRepository _repository;

    public InMemoryTodoRepositoryTests()
    {
        _repository = new InMemoryTodoRepository();
    }

    [Fact]
    public async Task AddAsync_WithValidItem_ReturnsItemWithId()
    {
        // Arrange
        var item = TodoItemFactory.Create("Test Todo");

        // Act
        var result = await _repository.AddAsync(item);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(item.Id);
        result.Title.Should().Be("Test Todo");
    }

    [Fact]
    public async Task AddAsync_WithNullItem_ThrowsArgumentNullException()
    {
        // Act & Assert
        var act = async () => await _repository.AddAsync(null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task AddAsync_WithDuplicateId_ThrowsInvalidOperationException()
    {
        // Arrange
        var item = TodoItemFactory.Create("Test");
        await _repository.AddAsync(item);

        // Act & Assert
        var act = async () => await _repository.AddAsync(item);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Item with ID {item.Id} already exists.");
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllItems()
    {
        // Arrange
        var item1 = TodoItemFactory.Create("Item 1");
        var item2 = TodoItemFactory.Create("Item 2");
        await _repository.AddAsync(item1);
        await _repository.AddAsync(item2);

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(x => x.Id == item1.Id);
        result.Should().Contain(x => x.Id == item2.Id);
    }

    [Fact]
    public async Task GetAllAsync_WhenEmpty_ReturnsEmptyCollection()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsItemsOrderedByCreatedAtDescending()
    {
        // Arrange
        var item1 = TodoItemFactory.Create("First");
        await Task.Delay(10); // Ensure different timestamps
        var item2 = TodoItemFactory.Create("Second");
        await Task.Delay(10);
        var item3 = TodoItemFactory.Create("Third");

        await _repository.AddAsync(item1);
        await _repository.AddAsync(item2);
        await _repository.AddAsync(item3);

        // Act
        var result = (await _repository.GetAllAsync()).ToList();

        // Assert
        result.Should().HaveCount(3);
        result[0].Title.Should().Be("Third");
        result[1].Title.Should().Be("Second");
        result[2].Title.Should().Be("First");
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ReturnsItem()
    {
        // Arrange
        var item = TodoItemFactory.Create("Test");
        await _repository.AddAsync(item);

        // Act
        var result = await _repository.GetByIdAsync(item.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(item.Id);
        result.Title.Should().Be("Test");
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingId_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_WithExistingItem_UpdatesSuccessfully()
    {
        // Arrange
        var item = TodoItemFactory.Create("Original");
        await _repository.AddAsync(item);

        var updatedItem = item with { Title = "Updated" };

        // Act
        var result = await _repository.UpdateAsync(updatedItem);

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be("Updated");

        var retrieved = await _repository.GetByIdAsync(item.Id);
        retrieved!.Title.Should().Be("Updated");
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistingItem_ReturnsNull()
    {
        // Arrange
        var item = TodoItemFactory.Create("Test");

        // Act
        var result = await _repository.UpdateAsync(item);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_WithNullItem_ThrowsArgumentNullException()
    {
        // Act & Assert
        var act = async () => await _repository.UpdateAsync(null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task DeleteAsync_WithExistingId_RemovesItem()
    {
        // Arrange
        var item = TodoItemFactory.Create("Test");
        await _repository.AddAsync(item);

        // Act
        var result = await _repository.DeleteAsync(item.Id);

        // Assert
        result.Should().BeTrue();

        var retrieved = await _repository.GetByIdAsync(item.Id);
        retrieved.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistingId_ReturnsFalse()
    {
        // Act
        var result = await _repository.DeleteAsync(Guid.NewGuid());

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ConcurrentAdds_HandledCorrectly()
    {
        // Arrange
        var tasks = Enumerable.Range(0, 100)
            .Select(i => Task.Run(async () =>
            {
                var item = TodoItemFactory.Create($"Item {i}");
                await _repository.AddAsync(item);
            }));

        // Act
        await Task.WhenAll(tasks);

        // Assert
        var allItems = await _repository.GetAllAsync();
        allItems.Should().HaveCount(100);
    }
}
