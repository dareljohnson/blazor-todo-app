using BlazorTodo.Models;
using BlazorTodo.Repositories;
using BlazorTodo.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BlazorTodo.Tests.Services;

public class TodoServiceTests
{
    private readonly Mock<ITodoRepository> _repositoryMock;
    private readonly Mock<ILogger<TodoService>> _loggerMock;
    private readonly TodoService _service;

    public TodoServiceTests()
    {
        _repositoryMock = new Mock<ITodoRepository>();
        _loggerMock = new Mock<ILogger<TodoService>>();
        _service = new TodoService(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public void Constructor_WithNullRepository_ThrowsArgumentNullException()
    {
        // Act & Assert
        var act = () => new TodoService(null!, _loggerMock.Object);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("repository");
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        var act = () => new TodoService(_repositoryMock.Object, null!);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("logger");
    }

    [Fact]
    public async Task GetAllAsync_CallsRepository()
    {
        // Arrange
        var items = new[] { TodoItemFactory.Create("Test 1"), TodoItemFactory.Create("Test 2") };
        _repositoryMock.Setup(x => x.GetAllAsync(default)).ReturnsAsync(items);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        _repositoryMock.Verify(x => x.GetAllAsync(default), Times.Once);
    }

    [Fact]
    public async Task GetActiveAsync_ReturnsOnlyIncompleteItems()
    {
        // Arrange
        var item1 = TodoItemFactory.Create("Active");
        var item2 = TodoItemFactory.Create("Completed") with { IsCompleted = true };
        var item3 = TodoItemFactory.Create("Also Active");

        _repositoryMock.Setup(x => x.GetAllAsync(default))
            .ReturnsAsync(new[] { item1, item2, item3 });

        // Act
        var result = (await _service.GetActiveAsync()).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(x => x.IsCompleted.Should().BeFalse());
        result.Should().Contain(x => x.Title == "Active");
        result.Should().Contain(x => x.Title == "Also Active");
    }

    [Fact]
    public async Task GetCompletedAsync_ReturnsOnlyCompletedItems()
    {
        // Arrange
        var item1 = TodoItemFactory.Create("Active");
        var item2 = TodoItemFactory.Create("Completed") with { IsCompleted = true };
        var item3 = TodoItemFactory.Create("Also Completed") with { IsCompleted = true };

        _repositoryMock.Setup(x => x.GetAllAsync(default))
            .ReturnsAsync(new[] { item1, item2, item3 });

        // Act
        var result = (await _service.GetCompletedAsync()).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(x => x.IsCompleted.Should().BeTrue());
        result.Should().Contain(x => x.Title == "Completed");
        result.Should().Contain(x => x.Title == "Also Completed");
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingItem_ReturnsDto()
    {
        // Arrange
        var item = TodoItemFactory.Create("Test");
        _repositoryMock.Setup(x => x.GetByIdAsync(item.Id, default)).ReturnsAsync(item);

        // Act
        var result = await _service.GetByIdAsync(item.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(item.Id);
        result.Title.Should().Be("Test");
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingItem_ReturnsNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repositoryMock.Setup(x => x.GetByIdAsync(id, default)).ReturnsAsync((TodoItem?)null);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_WithValidDto_CreatesItem()
    {
        // Arrange
        var dto = new TodoItemDto
        {
            Title = "New Todo",
            Description = "Description",
            Priority = TodoPriority.High
        };

        _repositoryMock.Setup(x => x.AddAsync(It.IsAny<TodoItem>(), default))
            .ReturnsAsync((TodoItem item, CancellationToken _) => item);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("New Todo");
        result.Description.Should().Be("Description");
        result.Priority.Should().Be(TodoPriority.High);
        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<TodoItem>(), default), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithNullDto_ThrowsArgumentNullException()
    {
        // Act & Assert
        var act = async () => await _service.CreateAsync(null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task CreateAsync_WithEmptyTitle_ThrowsArgumentException()
    {
        // Arrange
        var dto = new TodoItemDto { Title = "" };

        // Act & Assert
        var act = async () => await _service.CreateAsync(dto);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Title is required.*");
    }

    [Fact]
    public async Task UpdateAsync_WithExistingItem_Updates()
    {
        // Arrange
        var existingItem = TodoItemFactory.Create("Original");
        var dto = TodoItemDto.FromTodoItem(existingItem);
        dto.Title = "Updated";

        _repositoryMock.Setup(x => x.GetByIdAsync(dto.Id, default)).ReturnsAsync(existingItem);
        _repositoryMock.Setup(x => x.UpdateAsync(It.IsAny<TodoItem>(), default))
            .ReturnsAsync((TodoItem item, CancellationToken _) => item);

        // Act
        var result = await _service.UpdateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be("Updated");
        _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<TodoItem>(), default), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistingItem_ReturnsNull()
    {
        // Arrange
        var dto = new TodoItemDto { Id = Guid.NewGuid(), Title = "Test" };
        _repositoryMock.Setup(x => x.GetByIdAsync(dto.Id, default)).ReturnsAsync((TodoItem?)null);

        // Act
        var result = await _service.UpdateAsync(dto);

        // Assert
        result.Should().BeNull();
        _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<TodoItem>(), default), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WithEmptyTitle_ThrowsArgumentException()
    {
        // Arrange
        var dto = new TodoItemDto { Id = Guid.NewGuid(), Title = "" };

        // Act & Assert
        var act = async () => await _service.UpdateAsync(dto);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Title is required.*");
    }

    [Fact]
    public async Task ToggleCompleteAsync_ChangesCompletionStatus()
    {
        // Arrange
        var item = TodoItemFactory.Create("Test");
        _repositoryMock.Setup(x => x.GetByIdAsync(item.Id, default)).ReturnsAsync(item);
        _repositoryMock.Setup(x => x.UpdateAsync(It.IsAny<TodoItem>(), default))
            .ReturnsAsync((TodoItem i, CancellationToken _) => i);

        // Act
        var result = await _service.ToggleCompleteAsync(item.Id);

        // Assert
        result.Should().NotBeNull();
        result!.IsCompleted.Should().BeTrue();
        result.CompletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task ToggleCompleteAsync_WhenAlreadyCompleted_SetsToIncomplete()
    {
        // Arrange
        var item = TodoItemFactory.Create("Test") with
        {
            IsCompleted = true,
            CompletedAt = DateTime.UtcNow
        };

        _repositoryMock.Setup(x => x.GetByIdAsync(item.Id, default)).ReturnsAsync(item);
        _repositoryMock.Setup(x => x.UpdateAsync(It.IsAny<TodoItem>(), default))
            .ReturnsAsync((TodoItem i, CancellationToken _) => i);

        // Act
        var result = await _service.ToggleCompleteAsync(item.Id);

        // Assert
        result.Should().NotBeNull();
        result!.IsCompleted.Should().BeFalse();
        result.CompletedAt.Should().BeNull();
    }

    [Fact]
    public async Task ToggleCompleteAsync_SetsCompletedAtTimestamp()
    {
        // Arrange
        var item = TodoItemFactory.Create("Test");
        _repositoryMock.Setup(x => x.GetByIdAsync(item.Id, default)).ReturnsAsync(item);
        _repositoryMock.Setup(x => x.UpdateAsync(It.IsAny<TodoItem>(), default))
            .ReturnsAsync((TodoItem i, CancellationToken _) => i);

        var beforeToggle = DateTime.UtcNow;

        // Act
        var result = await _service.ToggleCompleteAsync(item.Id);

        // Assert
        result!.CompletedAt.Should().NotBeNull();
        result.CompletedAt!.Value.Should().BeOnOrAfter(beforeToggle);
        result.CompletedAt.Value.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task ToggleCompleteAsync_WithNonExistingItem_ReturnsNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repositoryMock.Setup(x => x.GetByIdAsync(id, default)).ReturnsAsync((TodoItem?)null);

        // Act
        var result = await _service.ToggleCompleteAsync(id);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_CallsRepository()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repositoryMock.Setup(x => x.DeleteAsync(id, default)).ReturnsAsync(true);

        // Act
        var result = await _service.DeleteAsync(id);

        // Assert
        result.Should().BeTrue();
        _repositoryMock.Verify(x => x.DeleteAsync(id, default), Times.Once);
    }
}
