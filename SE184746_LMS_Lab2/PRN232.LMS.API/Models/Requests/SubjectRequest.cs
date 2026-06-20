using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.API.Models.Requests;

public class CreateSubjectRequest
{
    [Required(ErrorMessage = "SubjectCode is required")]
    [StringLength(20, ErrorMessage = "SubjectCode cannot exceed 20 characters")]
    [RegularExpression(@"^[A-Z]{2,4}\d{3,4}$",
        ErrorMessage = "SubjectCode must be 2-4 uppercase letters followed by 3-4 digits (e.g. PRN231, SE401).")]
    public string SubjectCode { get; set; } = null!;

    [Required(ErrorMessage = "SubjectName is required")]
    [StringLength(200, ErrorMessage = "SubjectName cannot exceed 200 characters")]
    public string SubjectName { get; set; } = null!;

    [Required(ErrorMessage = "Credit is required")]
    [Range(1, 10, ErrorMessage = "Credit must be between 1 and 10")]
    public int Credit { get; set; }
}

public class UpdateSubjectRequest
{
    [Required(ErrorMessage = "SubjectCode is required")]
    [StringLength(20, ErrorMessage = "SubjectCode cannot exceed 20 characters")]
    [RegularExpression(@"^[A-Z]{2,4}\d{3,4}$",
        ErrorMessage = "SubjectCode must be 2-4 uppercase letters followed by 3-4 digits (e.g. PRN231, SE401).")]
    public string SubjectCode { get; set; } = null!;

    [Required(ErrorMessage = "SubjectName is required")]
    [StringLength(200, ErrorMessage = "SubjectName cannot exceed 200 characters")]
    public string SubjectName { get; set; } = null!;

    [Required(ErrorMessage = "Credit is required")]
    [Range(1, 10, ErrorMessage = "Credit must be between 1 and 10")]
    public int Credit { get; set; }
}
