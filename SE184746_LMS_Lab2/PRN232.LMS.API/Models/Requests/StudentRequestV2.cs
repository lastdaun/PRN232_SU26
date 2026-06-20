using System.ComponentModel.DataAnnotations;
using PRN232.LMS.API.Validators;

namespace PRN232.LMS.API.Models.Requests;

/// <summary>V2 request for creating a student – StudentCode is required in this version.</summary>
public class CreateStudentRequestV2
{
    [Required(ErrorMessage = "FullName is required")]
    [StringLength(100, ErrorMessage = "FullName cannot exceed 100 characters")]
    public string FullName { get; set; } = null!;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email is invalid")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "DateOfBirth is required")]
    public DateTime DateOfBirth { get; set; }

    [Required(ErrorMessage = "StudentCode is required in v2")]
    [FptuStudentCode]
    public string StudentCode { get; set; } = null!;

    [Phone(ErrorMessage = "PhoneNumber is invalid")]
    [StringLength(20, ErrorMessage = "PhoneNumber cannot exceed 20 characters")]
    public string? PhoneNumber { get; set; }
}

/// <summary>V2 request for updating a student – StudentCode is required in this version.</summary>
public class UpdateStudentRequestV2
{
    [Required(ErrorMessage = "FullName is required")]
    [StringLength(100, ErrorMessage = "FullName cannot exceed 100 characters")]
    public string FullName { get; set; } = null!;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email is invalid")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "DateOfBirth is required")]
    public DateTime DateOfBirth { get; set; }

    [Required(ErrorMessage = "StudentCode is required in v2")]
    [FptuStudentCode]
    public string StudentCode { get; set; } = null!;

    [Phone(ErrorMessage = "PhoneNumber is invalid")]
    [StringLength(20, ErrorMessage = "PhoneNumber cannot exceed 20 characters")]
    public string? PhoneNumber { get; set; }
}
