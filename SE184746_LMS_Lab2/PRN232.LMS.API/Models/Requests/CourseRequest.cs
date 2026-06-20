using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.API.Models.Requests;

public class CreateCourseRequest
{
    [Required(ErrorMessage = "CourseName is required")]
    [StringLength(200, ErrorMessage = "CourseName cannot exceed 200 characters")]
    public string CourseName { get; set; } = null!;

    [Required(ErrorMessage = "SemesterId is required")]
    [Range(1, int.MaxValue, ErrorMessage = "SemesterId must be a positive number")]
    public int SemesterId { get; set; }

    [Required(ErrorMessage = "SubjectId is required")]
    [Range(1, int.MaxValue, ErrorMessage = "SubjectId must be a positive number")]
    public int SubjectId { get; set; }
}

public class UpdateCourseRequest
{
    [Required(ErrorMessage = "CourseName is required")]
    [StringLength(200, ErrorMessage = "CourseName cannot exceed 200 characters")]
    public string CourseName { get; set; } = null!;

    [Required(ErrorMessage = "SemesterId is required")]
    [Range(1, int.MaxValue, ErrorMessage = "SemesterId must be a positive number")]
    public int SemesterId { get; set; }

    [Required(ErrorMessage = "SubjectId is required")]
    [Range(1, int.MaxValue, ErrorMessage = "SubjectId must be a positive number")]
    public int SubjectId { get; set; }
}
