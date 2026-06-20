using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.API.Models.Requests;

public class CreateSemesterRequest
{
    [Required(ErrorMessage = "SemesterName is required")]
    [StringLength(100, ErrorMessage = "SemesterName cannot exceed 100 characters")]
    public string SemesterName { get; set; } = null!;

    [Required(ErrorMessage = "StartDate is required")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "EndDate is required")]
    public DateTime EndDate { get; set; }
}

public class UpdateSemesterRequest
{
    [Required(ErrorMessage = "SemesterName is required")]
    [StringLength(100, ErrorMessage = "SemesterName cannot exceed 100 characters")]
    public string SemesterName { get; set; } = null!;

    [Required(ErrorMessage = "StartDate is required")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "EndDate is required")]
    public DateTime EndDate { get; set; }
}
