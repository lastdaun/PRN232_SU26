namespace PRN232.LMS.Services.Models.Responses;

public sealed class EnrollmentResponse
{
    public int EnrollmentId { get; init; }
    public int StudentId { get; init; }
    public int CourseId { get; init; }
    public DateTime EnrollDate { get; init; }
    public string Status { get; init; } = string.Empty;

    public StudentResponse? Student { get; init; }
    public CourseResponse? Course { get; init; }
}

