namespace PRN232.LMS.Services.Models.Business;

public sealed class SemesterBusiness
{
    public int SemesterId { get; init; }
    public string SemesterName { get; init; } = string.Empty;
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }

    public IReadOnlyList<CourseBusiness>? Courses { get; init; }
}
