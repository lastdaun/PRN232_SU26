namespace PRN232.LMS.Services.Models.Common;

/// <summary>Paged list payload returned inside ApiResponse.Data.</summary>
/// <typeparam name="T">Item type.</typeparam>
public sealed class PagedResult<T>
{
    /// <summary>Current page items.</summary>
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();

    /// <summary>Pagination metadata.</summary>
    public PaginationMetadata Pagination { get; init; } = new();
}
