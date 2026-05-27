namespace PRN232.LMS.Services.Models.Requests;

public sealed class GetSemesterByIdRequest : GetByIdRequestBase
{
    protected override IReadOnlyList<string> GetDefaultExpandTokens() => ["courses", "courses.subject"];
}
