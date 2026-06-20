using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.API.Models.Requests;

/// <summary>Query params for GET /api/v{version}/courses/{id}/enrollments</summary>
public class CourseEnrollmentsQueryRequest
{
    [FromQuery(Name = "search")]
    public string? Search { get; set; }

    [FromQuery(Name = "sort")]
    public string? Sort { get; set; }

    [FromQuery(Name = "page")]
    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
    public int Page { get; set; } = 1;

    [FromQuery(Name = "size")]
    [Range(1, 100, ErrorMessage = "Size must be between 1 and 100")]
    public int Size { get; set; } = 10;

    /// <summary>Include related resources, e.g. student,course</summary>
    [FromQuery(Name = "expand")]
    public string? Expand { get; set; }
}
