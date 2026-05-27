namespace PRN232.LMS.Services.Models.Common;

/// <summary>Pagination details for list endpoints.</summary>
public sealed class PaginationMetadata
{
    /// <summary>Current page number (1-based).</summary>
    public int Page { get; init; }

    /// <summary>Page size.</summary>
    public int PageSize { get; init; }

    /// <summary>Total number of items across all pages.</summary>
    public int TotalItems { get; init; }

    /// <summary>Total number of pages.</summary>
    public int TotalPages { get; init; }
}
