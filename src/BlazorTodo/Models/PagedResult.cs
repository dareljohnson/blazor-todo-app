namespace BlazorTodo.Models;

/// <summary>
/// Represents a paged result with metadata.
/// </summary>
/// <typeparam name="T">The type of items in the page.</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// The items on the current page.
    /// </summary>
    public IEnumerable<T> Items { get; init; } = Enumerable.Empty<T>();

    /// <summary>
    /// The current page number (1-based).
    /// </summary>
    public int PageNumber { get; init; }

    /// <summary>
    /// The number of items per page.
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// The total number of items across all pages.
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// The total number of pages.
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>
    /// Whether there is a previous page.
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Whether there is a next page.
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;
}
