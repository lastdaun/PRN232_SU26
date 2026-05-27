using System.ComponentModel.DataAnnotations;
using PRN232.LMS.Services.Helpers;

namespace PRN232.LMS.Services.Models.Requests;

/// <summary>
/// Common query parameters for paged list endpoints.
/// </summary>
public class PagedListRequest
{
    /// <summary>Keyword filter applied to searchable fields.</summary>
    public string? Search { get; init; }

    /// <summary>Comma-separated sort fields. Prefix with '-' for descending (e.g. fullName,-dateOfBirth).</summary>
    public string? Sort { get; init; }

    /// <summary>Page number (1-based).</summary>
    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than or equal to 1.")]
    public int Page { get; init; } = 1;

    /// <summary>Number of items per page (1-100).</summary>
    [Range(1, 100, ErrorMessage = "Size must be between 1 and 100.")]
    public int Size { get; init; } = 10;

    /// <summary>Comma-separated response fields to include (e.g. studentId,fullName,email).</summary>
    [MaxLength(500, ErrorMessage = "Fields must not exceed 500 characters.")]
    public string? Fields { get; init; }

    /// <summary>Comma-separated related resources to expand (e.g. student,course).</summary>
    [MaxLength(500, ErrorMessage = "Expand must not exceed 500 characters.")]
    public string? Expand { get; init; }

    public IReadOnlyList<string> GetExpandTokens() => QueryStringHelper.ParseCsv(Expand);

    public IReadOnlyList<string> GetFieldTokens() => QueryStringHelper.ParseCsv(Fields);

    public IReadOnlyList<string> GetSortTokens() => QueryStringHelper.ParseCsv(Sort);

    public (int Page, int Size) GetPaging() => (Page, Size);
}
