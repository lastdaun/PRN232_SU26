namespace PRN232.LMS.Services.Models.Requests;

public sealed class GetEnrollmentByIdRequest : GetByIdRequestBase
{
    protected override IReadOnlyList<string> GetDefaultExpandTokens() =>
    [
        "student",
        "course",
        "course.semester",
        "course.subject"
    ];
}
