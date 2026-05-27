namespace PRN232.LMS.Services.Models.Responses;

public sealed class SubjectResponse
{
    public int SubjectId { get; init; }
    public string SubjectCode { get; init; } = string.Empty;
    public string SubjectName { get; init; } = string.Empty;
    public int Credit { get; init; }

    public IReadOnlyList<CourseResponse>? Courses { get; init; }
}

