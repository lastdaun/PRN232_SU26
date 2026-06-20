namespace PRN232.LMS.API.Helpers;

public static class ExpandQueryHelper
{
    public static string[]? Parse(string? expand) =>
        expand?.Split(',', StringSplitOptions.RemoveEmptyEntries)
               .Select(e => e.Trim())
               .ToArray();

    public static bool Includes(string[]? parts, string token) =>
        parts?.Contains(token, StringComparer.OrdinalIgnoreCase) == true;

    public static bool IncludesEnrollments(string[]? parts) => Includes(parts, "enrollments");
}
