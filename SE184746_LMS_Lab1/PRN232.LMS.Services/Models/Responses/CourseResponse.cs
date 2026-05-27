namespace PRN232.LMS.Services.Models.Responses;

public sealed class CourseResponse
{
    public int CourseId { get; init; }
    public string CourseName { get; init; } = string.Empty;
    public int SemesterId { get; init; }
    public int SubjectId { get; init; }

    public SemesterResponse? Semester { get; init; }
    public SubjectResponse? Subject { get; init; }

    public IReadOnlyList<EnrollmentResponse>? Enrollments { get; init; }
}

