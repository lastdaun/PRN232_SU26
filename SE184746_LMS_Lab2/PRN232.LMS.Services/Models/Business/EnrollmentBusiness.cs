namespace PRN232.LMS.Services.Models.Business;

public class EnrollmentBusiness
{
    public int EnrollmentId { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public DateTime EnrollDate { get; set; }
    public string Status { get; set; } = null!;
    public StudentBusiness? Student { get; set; }
    public CourseBusiness? Course { get; set; }
}
