namespace PRN232.LMS.Services.Models.Business;

public sealed class CourseBusiness
{
    public int CourseId { get; init; }
    public string CourseName { get; init; } = string.Empty;
    public int SemesterId { get; init; }
    public int SubjectId { get; init; }

    public SemesterBusiness? Semester { get; init; }
    public SubjectBusiness? Subject { get; init; }

    public IReadOnlyList<EnrollmentBusiness>? Enrollments { get; init; }
}
