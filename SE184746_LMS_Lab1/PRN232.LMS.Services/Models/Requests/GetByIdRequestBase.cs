using PRN232.LMS.Services.Helpers;

namespace PRN232.LMS.Services.Models.Requests;

public abstract class GetByIdRequestBase
{
    public int Id { get; init; }
    public string? Expand { get; init; }

    public IReadOnlyList<string> GetEffectiveExpandTokens()
    {
        var tokens = QueryStringHelper.ParseCsv(Expand);
        return tokens.Count > 0 ? tokens : GetDefaultExpandTokens();
    }

    protected abstract IReadOnlyList<string> GetDefaultExpandTokens();
}
