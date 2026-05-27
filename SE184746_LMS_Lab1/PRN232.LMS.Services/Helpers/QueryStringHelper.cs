namespace PRN232.LMS.Services.Helpers;

public static class QueryStringHelper
{
    public static List<string> ParseCsv(string? csv)
    {
        if (string.IsNullOrWhiteSpace(csv))
        {
            return new List<string>();
        }

        return csv
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();
    }

    public static HashSet<string> ToExpandSet(IEnumerable<string> expand)
        => new(expand.Select(x => x.Trim().ToLowerInvariant()));
}
