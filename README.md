# Blazor Todo App

A beautiful, modern todo application built with **ASP.NET Core Blazor Server** and **.NET 8.0**. This application demonstrates clean architecture, SOLID principles, and comprehensive testing.

## Features

✨ **Modern UI**
- Beautiful gradient header with custom styling
- Responsive design for all screen sizes
- Smooth animations and transitions
- Priority badges with color coding
- Due date indicators with overdue warnings

📝 **Full CRUD Operations**
- Create todos with title, description, priority, and due date
- Update todos with inline editing (double-click to edit)
- Toggle completion status with visual feedback
- Delete todos with confirmation

🔍 **Smart Filtering**
- View all todos
- Filter by active (incomplete) todos
- Filter by completed todos
- Real-time count badges

📄 **Pagination**
- Efficient page-based loading (10 items per page)
- Previous/Next navigation with disabled states
- Page indicator showing current page and total pages
- Item counter showing visible items and total count
- Mobile-responsive pagination controls
- Handles large datasets without performance issues

⚡ **Priority Management**
- Three priority levels: Low, Medium, High
- Color-coded visual indicators (🟢 🟡 🔴)
- Priority-based border colors

📅 **Due Date Tracking**
- Optional due dates for tasks
- Visual indicators for:
  - Overdue tasks (red)
  - Due today (yellow)
  - Due soon (blue)

## Architecture

The application follows **Clean Architecture** principles with clear separation of concerns:

```
BlazorTodo/
├── Models/              # Domain entities and DTOs
│   ├── TodoItem.cs      # Immutable record for todo items
│   ├── TodoItemDto.cs   # Data transfer object
│   ├── TodoPriority.cs  # Priority enum
│   └── PagedResult.cs   # Pagination metadata wrapper
├── Repositories/        # Data access layer
│   ├── ITodoRepository.cs
│   └── InMemoryTodoRepository.cs
├── Services/            # Business logic layer
│   ├── ITodoService.cs
│   └── TodoService.cs
└── Components/          # Presentation layer
    ├── Layout/
    │   └── MainLayout.razor
    ├── Pages/
    │   └── Home.razor
    └── Shared/
        ├── TodoForm.razor
        ├── TodoList.razor
        └── TodoItemComponent.razor
```

### Design Patterns Used

- **Repository Pattern**: Abstracts data access
- **Factory Pattern**: TodoItemFactory for creating validated instances
- **Dependency Injection**: Built-in DI container for service management
- **Component Composition**: Reusable Blazor components

## Technology Stack

- **Framework**: ASP.NET Core Blazor Server
- **Target**: .NET 8.0 (LTS)
- **Testing**: xUnit, Moq, FluentAssertions
- **Language**: C# 10+ with modern features
  - File-scoped namespaces
  - Record types
  - Nullable reference types
  - Pattern matching

## Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- Any modern IDE (Visual Studio 2022, VS Code, Rider)

### Installation

1. Clone or download this repository

2. Navigate to the solution directory:
   ```powershell
   cd c:\development\dotnet_apps\blazor-todo
   ```

3. Restore dependencies:
   ```powershell
   dotnet restore
   ```

4. Build the solution:
   ```powershell
   dotnet build -c Release
   ```

5. Run tests:
   ```powershell
   dotnet test -c Release
   ```

6. Run the application:
   ```powershell
   cd src\BlazorTodo
   dotnet run
   ```

7. Open your browser and navigate to:
   - HTTPS: `https://localhost:7XXX` (port shown in console)
   - HTTP: `http://localhost:5XXX`

## Usage

### Creating a Todo

1. Fill in the "Add New Task" form:
   - **Title**: Required - what needs to be done
   - **Description**: Optional - additional details
   - **Priority**: Low, Medium, or High
   - **Due Date**: Optional - when it's due

2. Click "➕ Add Task"

### Managing Todos

- **Complete/Uncomplete**: Click the checkbox next to any todo
- **Edit**: Double-click the todo content or click the edit button (✏️)
- **Delete**: Click the delete button (🗑️) - a beautiful custom modal dialog will appear to confirm the deletion
- **Filter**: Use the filter buttons (All, Active, Completed) to view specific todos

### Keyboard Shortcuts (while editing)

- **Enter**: Save changes
- **Escape**: Cancel editing

## Testing

The application includes comprehensive unit tests covering:

### Test Coverage

- ✅ **TodoItemFactoryTests** (15 tests)
  - Validation of required fields
  - Whitespace handling
  - Update operations
  - Edge cases

- ✅ **InMemoryTodoRepositoryTests** (14 tests)
  - CRUD operations
  - Null handling
  - Thread safety
  - Concurrent operations

- ✅ **TodoServiceTests** (17 tests)
  - Business logic validation
  - Filtering (active/completed)
  - Toggle completion with timestamps
  - Error handling

### Running Tests

```powershell
# Run all tests
dotnet test

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run with coverage (if configured)
dotnet test /p:CollectCoverage=true
```

## Data Persistence

The current implementation uses **in-memory storage** (ConcurrentDictionary). Data is lost when the application restarts.

### Extending to Database

To add database persistence:

1. Install Entity Framework Core:
   ```powershell
   dotnet add package Microsoft.EntityFrameworkCore.SqlServer
   ```

2. Create a new repository implementation:
   ```csharp
   public class EfTodoRepository : ITodoRepository
   {
       // Implement using DbContext
   }
   ```

3. Update service registration in [Program.cs](src/BlazorTodo/Program.cs):
   ```csharp
   builder.Services.AddDbContext<TodoDbContext>();
   builder.Services.AddScoped<ITodoRepository, EfTodoRepository>();
   ```

## Project Structure

```
blazor-todo/
├── src/
│   └── BlazorTodo/           # Main Blazor application
│       ├── Components/       # Blazor components
│       ├── Models/           # Domain models
│       ├── Repositories/     # Data access
│       ├── Services/         # Business logic
│       └── wwwroot/          # Static files & CSS
├── tests/
│   └── BlazorTodo.Tests/     # Unit tests
│       ├── Models/
│       ├── Repositories/
│       └── Services/
├── BlazorTodo.sln            # Solution file
├── README.md                 # This file
└── CHANGELOG.md              # Version history
```

## Code Quality

### C# Best Practices

- **Immutable Records**: Domain models use C# records
- **Async/Await**: All I/O operations are async
- **Nullable Reference Types**: Enabled for null safety
- **Dependency Injection**: Interface-based DI throughout
- **XML Documentation**: All public APIs documented

### SOLID Principles

- **Single Responsibility**: Each class has one reason to change
- **Open/Closed**: Extensible through interfaces
- **Liskov Substitution**: Interface implementations are substitutable
- **Interface Segregation**: Focused, minimal interfaces
- **Dependency Inversion**: Depends on abstractions, not concretions

## Customization

### Changing Colors

Edit [wwwroot/app.css](src/BlazorTodo/wwwroot/app.css) CSS variables:

```css
:root {
    --primary-color: #4f46e5;     /* Main accent color */
    --success-color: #10b981;      /* Low priority / success */
    --warning-color: #f59e0b;      /* Medium priority */
    --danger-color: #ef4444;       /* High priority / delete */
}
```

### Adding Features

Some ideas for extensions:

- **User Authentication**: Add ASP.NET Core Identity
- **Categories/Tags**: Organize todos by category
- **Subtasks**: Nested todo items
- **Search**: Full-text search across todos
- **Export/Import**: JSON or CSV support
- **Notifications**: Browser notifications for due dates
- **Dark Mode**: Toggle between light/dark themes

## Contributing

This is a demonstration project. Feel free to fork and customize for your needs!

## License

This project is provided as-is for educational purposes.

## Troubleshooting

### Port Already in Use

If the default port is taken, specify a different port:

```powershell
dotnet run --urls "https://localhost:7123;http://localhost:5123"
```

### Tests Failing

Ensure you're in the solution root directory and run:

```powershell
dotnet restore
dotnet build
dotnet test
```

### Build Errors

Check that you have .NET 8.0 SDK installed:

```powershell
dotnet --version
```

Should show `8.0.xxx` or later.

## Support

For issues or questions, please check the [CHANGELOG.md](CHANGELOG.md) for version-specific information.

---

**Built with ❤️ using ASP.NET Core Blazor**
