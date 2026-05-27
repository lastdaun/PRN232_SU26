using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.Services.Models.Requests;

/// <summary>Request body for updating a course.</summary>
public sealed class UpdateCourseRequest
{
    [Required(ErrorMessage = "CourseName is required.")]
    [MaxLength(100, ErrorMessage = "CourseName must not exceed 100 characters.")]
    public string CourseName { get; init; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "SemesterId must be greater than 0.")]
    public int SemesterId { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "SubjectId must be greater than 0.")]
    public int SubjectId { get; init; }
}
