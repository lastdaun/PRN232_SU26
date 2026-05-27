using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.Services.Models.Requests;

/// <summary>Request body for updating an enrollment.</summary>
public sealed class UpdateEnrollmentRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "StudentId must be greater than 0.")]
    public int StudentId { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "CourseId must be greater than 0.")]
    public int CourseId { get; init; }

    [Required(ErrorMessage = "EnrollDate is required.")]
    public DateTime EnrollDate { get; init; }

    [Required(ErrorMessage = "Status is required.")]
    [MaxLength(20, ErrorMessage = "Status must not exceed 20 characters.")]
    public string Status { get; init; } = string.Empty;
}
