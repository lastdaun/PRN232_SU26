namespace PRN232.LMS.Services.Models.Responses;

public sealed class SemesterResponse
{
    public int SemesterId { get; init; }
    public string SemesterName { get; init; } = string.Empty;
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }

    public IReadOnlyList<CourseResponse>? Courses { get; init; }
}

