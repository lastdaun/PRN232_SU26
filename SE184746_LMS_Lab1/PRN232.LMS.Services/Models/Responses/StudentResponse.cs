namespace PRN232.LMS.Services.Models.Responses;

public sealed class StudentResponse
{
    public int StudentId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public DateTime DateOfBirth { get; init; }

    public IReadOnlyList<EnrollmentResponse>? Enrollments { get; init; }
}

