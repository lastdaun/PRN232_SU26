namespace PRN232.LMS.Services.Models.Business;

public sealed class EnrollmentBusiness
{
    public int EnrollmentId { get; init; }
    public int StudentId { get; init; }
    public int CourseId { get; init; }
    public DateTime EnrollDate { get; init; }
    public string Status { get; init; } = string.Empty;

    public StudentBusiness? Student { get; init; }
    public CourseBusiness? Course { get; init; }
}
