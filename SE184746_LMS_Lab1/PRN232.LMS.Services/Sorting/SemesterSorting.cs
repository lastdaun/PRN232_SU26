using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Services.Sorting;

public static class SemesterSorting
{
    public static IQueryable<Semester> Apply(IQueryable<Semester> query, IReadOnlyList<string> sortTokens)
    {
        if (sortTokens.Count == 0)
        {
            return query.OrderBy(s => s.SemesterId);
        }

        IOrderedQueryable<Semester>? ordered = null;

        foreach (var raw in sortTokens)
        {
            var desc = raw.StartsWith('-');
            var field = (desc ? raw[1..] : raw).Trim().ToLowerInvariant();
            ordered = ordered is null
                ? ApplyFirst(query, field, desc)
                : ApplyThen(ordered, field, desc);
        }

        return ordered ?? query.OrderBy(s => s.SemesterId);
    }

    private static IOrderedQueryable<Semester> ApplyFirst(IQueryable<Semester> query, string field, bool desc)
        => field switch
        {
            "semesterid" => desc ? query.OrderByDescending(s => s.SemesterId) : query.OrderBy(s => s.SemesterId),
            "semestername" => desc ? query.OrderByDescending(s => s.SemesterName) : query.OrderBy(s => s.SemesterName),
            "startdate" => desc ? query.OrderByDescending(s => s.StartDate) : query.OrderBy(s => s.StartDate),
            "enddate" => desc ? query.OrderByDescending(s => s.EndDate) : query.OrderBy(s => s.EndDate),
            _ => query.OrderBy(s => s.SemesterId)
        };

    private static IOrderedQueryable<Semester> ApplyThen(IOrderedQueryable<Semester> ordered, string field, bool desc)
        => field switch
        {
            "semesterid" => desc ? ordered.ThenByDescending(s => s.SemesterId) : ordered.ThenBy(s => s.SemesterId),
            "semestername" => desc ? ordered.ThenByDescending(s => s.SemesterName) : ordered.ThenBy(s => s.SemesterName),
            "startdate" => desc ? ordered.ThenByDescending(s => s.StartDate) : ordered.ThenBy(s => s.StartDate),
            "enddate" => desc ? ordered.ThenByDescending(s => s.EndDate) : ordered.ThenBy(s => s.EndDate),
            _ => ordered.ThenBy(s => s.SemesterId)
        };
}
