namespace PRN232.LMS.API.Models.Responses;

/// <summary>V2 student response – always includes StudentCode and PhoneNumber fields.</summary>
public class StudentResponseV2
{
    public int? StudentId { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? StudentCode { get; set; }
    public string? PhoneNumber { get; set; }
    public List<EnrollmentResponse>? Enrollments { get; set; }
}
