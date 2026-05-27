using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.Services.Models.Requests;

/// <summary>Request body for creating a subject.</summary>
public sealed class CreateSubjectRequest
{
    [Required(ErrorMessage = "SubjectCode is required.")]
    [MaxLength(20, ErrorMessage = "SubjectCode must not exceed 20 characters.")]
    public string SubjectCode { get; init; } = string.Empty;

    [Required(ErrorMessage = "SubjectName is required.")]
    [MaxLength(100, ErrorMessage = "SubjectName must not exceed 100 characters.")]
    public string SubjectName { get; init; } = string.Empty;

    [Range(1, 30, ErrorMessage = "Credit must be between 1 and 30.")]
    public int Credit { get; init; }
}
