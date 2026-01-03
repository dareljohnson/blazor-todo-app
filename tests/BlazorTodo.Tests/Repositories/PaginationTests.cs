using BlazorTodo.Models;
using BlazorTodo.Repositories;
using FluentAssertions;
using Xunit;

namespace BlazorTodo.Tests.Repositories;

public class PaginationTests
{
    private readonly InMemoryTodoRepository _repository;

    public PaginationTests()
    {
        _repository = new InMemoryTodoRepository();
    }

    [Fact]
    public async Task GetPagedAsync_ReturnsCorrectPageSize()
    {
        // Arrange
        for (int i = 0; i < 25; i++)
        {
            await _repository.AddAsync(TodoItemFactory.Create($"Todo {i}"));
        }

        // Act
        var result = await _repository.GetPagedAsync(pageNumber: 1, pageSize: 10);

        // Assert
        result.Items.Should().HaveCount(10);
        result.PageSize.Should().Be(10);
        result.PageNumber.Should().Be(1);
        result.TotalCount.Should().Be(25);
        result.TotalPages.Should().Be(3);
    }

    [Fact]
    public async Task GetPagedAsync_ReturnsCorrectPage()
    {
        // Arrange
        var todos = new List<TodoItem>();
        for (int i = 0; i < 25; i++)
        {
            var todo = TodoItemFactory.Create($"Todo {i}");
            await _repository.AddAsync(todo);
            todos.Add(todo);
        }

        // Act - Get second page
        var result = await _repository.GetPagedAsync(pageNumber: 2, pageSize: 10);

        // Assert
        result.Items.Should().HaveCount(10);
        result.PageNumber.Should().Be(2);
        result.HasPreviousPage.Should().BeTrue();
        result.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public async Task GetPagedAsync_LastPageHasCorrectCount()
    {
        // Arrange
        for (int i = 0; i < 25; i++)
        {
            await _repository.AddAsync(TodoItemFactory.Create($"Todo {i}"));
        }

        // Act - Get last page (should have 5 items)
        var result = await _repository.GetPagedAsync(pageNumber: 3, pageSize: 10);

        // Assert
        result.Items.Should().HaveCount(5);
        result.PageNumber.Should().Be(3);
        result.HasPreviousPage.Should().BeTrue();
        result.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public async Task GetPagedAsync_EmptyRepository_ReturnsEmptyPage()
    {
        // Act
        var result = await _repository.GetPagedAsync(pageNumber: 1, pageSize: 10);

        // Assert
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.TotalPages.Should().Be(0);
        result.HasPreviousPage.Should().BeFalse();
        result.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public async Task GetPagedAsync_InvalidPageNumber_DefaultsToPageOne()
    {
        // Arrange
        for (int i = 0; i < 15; i++)
        {
            await _repository.AddAsync(TodoItemFactory.Create($"Todo {i}"));
        }

        // Act
        var result = await _repository.GetPagedAsync(pageNumber: 0, pageSize: 10);

        // Assert
        result.PageNumber.Should().Be(1);
        result.Items.Should().HaveCount(10);
    }

    [Fact]
    public async Task GetPagedAsync_InvalidPageSize_DefaultsToTen()
    {
        // Arrange
        for (int i = 0; i < 15; i++)
        {
            await _repository.AddAsync(TodoItemFactory.Create($"Todo {i}"));
        }

        // Act
        var result = await _repository.GetPagedAsync(pageNumber: 1, pageSize: 0);

        // Assert
        result.PageSize.Should().Be(10);
        result.Items.Should().HaveCount(10);
    }

    [Fact]
    public async Task GetPagedAsync_OrdersByCreatedAtDescending()
    {
        // Arrange
        var firstTodo = TodoItemFactory.Create("First");
        var secondTodo = TodoItemFactory.Create("Second");
        var thirdTodo = TodoItemFactory.Create("Third");

        await _repository.AddAsync(firstTodo);
        await Task.Delay(10); // Ensure different timestamps
        await _repository.AddAsync(secondTodo);
        await Task.Delay(10);
        await _repository.AddAsync(thirdTodo);

        // Act
        var result = await _repository.GetPagedAsync(pageNumber: 1, pageSize: 10);

        // Assert
        var items = result.Items.ToList();
        items[0].Title.Should().Be("Third");  // Most recent first
        items[1].Title.Should().Be("Second");
        items[2].Title.Should().Be("First");
    }

    [Fact]
    public async Task GetPagedAsync_PageBeyondTotalPages_ReturnsEmptyPage()
    {
        // Arrange
        for (int i = 0; i < 15; i++)
        {
            await _repository.AddAsync(TodoItemFactory.Create($"Todo {i}"));
        }

        // Act - Request page 10 when only 2 pages exist
        var result = await _repository.GetPagedAsync(pageNumber: 10, pageSize: 10);

        // Assert
        result.Items.Should().BeEmpty();
        result.PageNumber.Should().Be(10);
        result.TotalCount.Should().Be(15);
        result.TotalPages.Should().Be(2);
        result.HasNextPage.Should().BeFalse();
    }
}
