using System.ComponentModel.DataAnnotations;
using PRN232.LMS.API.Validators;

namespace PRN232.LMS.API.Models.Requests;

public class CreateStudentRequest
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

    [FptuStudentCode]
    public string? StudentCode { get; set; }

    [Phone(ErrorMessage = "PhoneNumber is invalid")]
    [StringLength(20, ErrorMessage = "PhoneNumber cannot exceed 20 characters")]
    public string? PhoneNumber { get; set; }
}

public class UpdateStudentRequest
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

    [FptuStudentCode]
    public string? StudentCode { get; set; }

    [Phone(ErrorMessage = "PhoneNumber is invalid")]
    [StringLength(20, ErrorMessage = "PhoneNumber cannot exceed 20 characters")]
    public string? PhoneNumber { get; set; }
}
