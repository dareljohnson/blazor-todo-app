using BlazorTodo.Models;
using FluentAssertions;
using Xunit;

namespace BlazorTodo.Tests.Models;

public class TodoItemFactoryTests
{
    [Fact]
    public void Create_WithValidInputs_ReturnsValidTodoItem()
    {
        // Arrange
        var title = "Test Todo";
        var description = "Test Description";
        var dueDate = DateTime.UtcNow.AddDays(7);
        var priority = TodoPriority.High;

        // Act
        var result = TodoItemFactory.Create(title, description, dueDate, priority);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.Title.Should().Be(title);
        result.Description.Should().Be(description);
        result.DueDate.Should().Be(dueDate);
        result.Priority.Should().Be(priority);
        result.IsCompleted.Should().BeFalse();
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        result.CompletedAt.Should().BeNull();
    }

    [Fact]
    public void Create_WithEmptyTitle_ThrowsArgumentException()
    {
        // Act & Assert
        var act = () => TodoItemFactory.Create("");

        act.Should().Throw<ArgumentException>()
            .WithParameterName("title")
            .WithMessage("Title cannot be empty or whitespace.*");
    }

    [Fact]
    public void Create_WithWhitespaceTitle_ThrowsArgumentException()
    {
        // Act & Assert
        var act = () => TodoItemFactory.Create("   ");

        act.Should().Throw<ArgumentException>()
            .WithParameterName("title")
            .WithMessage("Title cannot be empty or whitespace.*");
    }

    [Fact]
    public void Create_WithNullTitle_ThrowsArgumentException()
    {
        // Act & Assert
        var act = () => TodoItemFactory.Create(null!);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("title");
    }

    [Fact]
    public void Create_WithPastDueDate_AcceptsDate()
    {
        // Arrange
        var pastDate = DateTime.UtcNow.AddDays(-7);

        // Act
        var result = TodoItemFactory.Create("Test", dueDate: pastDate);

        // Assert
        result.DueDate.Should().Be(pastDate);
    }

    [Fact]
    public void Create_TrimsWhitespaceFromTitle()
    {
        // Act
        var result = TodoItemFactory.Create("  Test Title  ");

        // Assert
        result.Title.Should().Be("Test Title");
    }

    [Fact]
    public void Create_TrimsWhitespaceFromDescription()
    {
        // Act
        var result = TodoItemFactory.Create("Test", "  Test Description  ");

        // Assert
        result.Description.Should().Be("Test Description");
    }

    [Fact]
    public void Create_WithEmptyDescription_SetsDescriptionToNull()
    {
        // Act
        var result = TodoItemFactory.Create("Test", "   ");

        // Assert
        result.Description.Should().BeNull();
    }

    [Fact]
    public void Update_WithValidInputs_UpdatesProperties()
    {
        // Arrange
        var original = TodoItemFactory.Create("Original", "Original Desc");
        var newTitle = "Updated Title";
        var newDescription = "Updated Description";
        var newDueDate = DateTime.UtcNow.AddDays(5);
        var newPriority = TodoPriority.High;

        // Act
        var result = TodoItemFactory.Update(original, newTitle, newDescription, newDueDate, newPriority);

        // Assert
        result.Id.Should().Be(original.Id);
        result.Title.Should().Be(newTitle);
        result.Description.Should().Be(newDescription);
        result.DueDate.Should().Be(newDueDate);
        result.Priority.Should().Be(newPriority);
    }

    [Fact]
    public void Update_WithNullExistingItem_ThrowsArgumentNullException()
    {
        // Act & Assert
        var act = () => TodoItemFactory.Update(null!, "New Title");

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Update_WithEmptyTitle_ThrowsArgumentException()
    {
        // Arrange
        var original = TodoItemFactory.Create("Original");

        // Act & Assert
        var act = () => TodoItemFactory.Update(original, "");

        act.Should().Throw<ArgumentException>()
            .WithParameterName("title");
    }

    [Fact]
    public void Update_WithNullTitle_KeepsOriginalTitle()
    {
        // Arrange
        var original = TodoItemFactory.Create("Original");

        // Act
        var result = TodoItemFactory.Update(original, description: "New Desc");

        // Assert
        result.Title.Should().Be("Original");
    }
}
