namespace PRN232.LMS.Services.Models.Business;

public sealed class SubjectBusiness
{
    public int SubjectId { get; init; }
    public string SubjectCode { get; init; } = string.Empty;
    public string SubjectName { get; init; } = string.Empty;
    public int Credit { get; init; }

    public IReadOnlyList<CourseBusiness>? Courses { get; init; }
}
