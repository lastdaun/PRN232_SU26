using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.API.Models.Requests;

public class CreateEnrollmentRequest
{
    [Required(ErrorMessage = "StudentId is required")]
    [Range(1, int.MaxValue, ErrorMessage = "StudentId must be a positive number")]
    public int StudentId { get; set; }

    [Required(ErrorMessage = "CourseId is required")]
    [Range(1, int.MaxValue, ErrorMessage = "CourseId must be a positive number")]
    public int CourseId { get; set; }

    [Required(ErrorMessage = "EnrollDate is required")]
    public DateTime EnrollDate { get; set; }

    [Required(ErrorMessage = "Status is required")]
    [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
    public string Status { get; set; } = null!;
}

public class UpdateEnrollmentRequest
{
    [Required(ErrorMessage = "StudentId is required")]
    [Range(1, int.MaxValue, ErrorMessage = "StudentId must be a positive number")]
    public int StudentId { get; set; }

    [Required(ErrorMessage = "CourseId is required")]
    [Range(1, int.MaxValue, ErrorMessage = "CourseId must be a positive number")]
    public int CourseId { get; set; }

    [Required(ErrorMessage = "EnrollDate is required")]
    public DateTime EnrollDate { get; set; }

    [Required(ErrorMessage = "Status is required")]
    [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
    public string Status { get; set; } = null!;
}

public class PatchEnrollmentStatusRequest
{
    [Required(ErrorMessage = "Status is required")]
    [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
    public string Status { get; set; } = null!;
}
