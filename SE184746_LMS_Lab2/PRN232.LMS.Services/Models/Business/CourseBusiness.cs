namespace PRN232.LMS.Services.Models.Business;

public class CourseBusiness
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = null!;
    public int SemesterId { get; set; }
    public int SubjectId { get; set; }
    public SemesterBusiness? Semester { get; set; }
    public SubjectBusiness? Subject { get; set; }
    public List<EnrollmentBusiness>? Enrollments { get; set; }
}
