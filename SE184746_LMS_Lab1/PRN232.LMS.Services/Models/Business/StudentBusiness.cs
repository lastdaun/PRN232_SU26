namespace PRN232.LMS.Services.Models.Business;

public sealed class StudentBusiness
{
    public int StudentId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public DateTime DateOfBirth { get; init; }

    public IReadOnlyList<EnrollmentBusiness>? Enrollments { get; init; }
}
