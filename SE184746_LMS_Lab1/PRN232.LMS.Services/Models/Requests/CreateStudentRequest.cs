using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.Services.Models.Requests;

/// <summary>Request body for creating a student.</summary>
public sealed class CreateStudentRequest
{
    [Required(ErrorMessage = "FullName is required.")]
    [MaxLength(100, ErrorMessage = "FullName must not exceed 100 characters.")]
    public string FullName { get; init; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email is not valid.")]
    [MaxLength(100, ErrorMessage = "Email must not exceed 100 characters.")]
    public string Email { get; init; } = string.Empty;

    [Required(ErrorMessage = "DateOfBirth is required.")]
    public DateTime DateOfBirth { get; init; }
}
