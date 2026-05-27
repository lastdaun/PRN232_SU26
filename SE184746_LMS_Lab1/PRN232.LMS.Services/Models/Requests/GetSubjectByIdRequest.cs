namespace PRN232.LMS.Services.Models.Requests;

public sealed class GetSubjectByIdRequest : GetByIdRequestBase
{
    protected override IReadOnlyList<string> GetDefaultExpandTokens() => ["courses", "courses.semester"];
}
