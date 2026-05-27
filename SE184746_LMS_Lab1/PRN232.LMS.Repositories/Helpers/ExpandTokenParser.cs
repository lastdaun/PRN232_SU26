namespace PRN232.LMS.Repositories.Helpers;

internal static class ExpandTokenParser
{
    public static HashSet<string> ToSet(IEnumerable<string> expand)
        => new(expand.Select(x => x.Trim().ToLowerInvariant()));
}
