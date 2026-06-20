namespace PRN232.LMS.Services.Models.Business;

public class StudentBusiness
{
    public int StudentId { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }
    public string? StudentCode { get; set; }
    public string? PhoneNumber { get; set; }
    public List<EnrollmentBusiness>? Enrollments { get; set; }
}
