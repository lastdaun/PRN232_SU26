namespace PRN232.LMS.Services.Models.Requests;

public sealed class GetStudentByIdRequest : GetByIdRequestBase
{
    protected override IReadOnlyList<string> GetDefaultExpandTokens() =>
    [
        "enrollments",
        "enrollments.course",
        "enrollments.course.semester",
        "enrollments.course.subject"
    ];
}
