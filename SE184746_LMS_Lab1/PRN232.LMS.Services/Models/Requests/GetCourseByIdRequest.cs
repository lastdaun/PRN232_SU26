namespace PRN232.LMS.Services.Models.Requests;

public sealed class GetCourseByIdRequest : GetByIdRequestBase
{
    protected override IReadOnlyList<string> GetDefaultExpandTokens() => ["semester", "subject"];
}
