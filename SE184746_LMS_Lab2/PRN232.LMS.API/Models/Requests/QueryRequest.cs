using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.API.Models.Requests;

public class QueryRequest
{
    [FromQuery(Name = "search")]
    public string? Search { get; set; }

    /// <summary>Comma-separated sort fields, e.g. fullName,-dateOfBirth</summary>
    [FromQuery(Name = "sort")]
    public string? Sort { get; set; }

    [FromQuery(Name = "page")]
    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
    public int Page { get; set; } = 1;

    [FromQuery(Name = "size")]
    [Range(1, 100, ErrorMessage = "Size must be between 1 and 100")]
    public int Size { get; set; } = 10;

    /// <summary>Comma-separated response fields, e.g. studentId,fullName</summary>
    [FromQuery(Name = "fields")]
    public string? Fields { get; set; }

    /// <summary>Comma-separated related resources to expand, e.g. enrollments,semester</summary>
    [FromQuery(Name = "expand")]
    public string? Expand { get; set; }
}
