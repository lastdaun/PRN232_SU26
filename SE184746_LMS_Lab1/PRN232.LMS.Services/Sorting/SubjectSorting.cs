using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Services.Sorting;

public static class SubjectSorting
{
    public static IQueryable<Subject> Apply(IQueryable<Subject> query, IReadOnlyList<string> sortTokens)
    {
        if (sortTokens.Count == 0)
        {
            return query.OrderBy(s => s.SubjectId);
        }

        IOrderedQueryable<Subject>? ordered = null;

        foreach (var raw in sortTokens)
        {
            var desc = raw.StartsWith('-');
            var field = (desc ? raw[1..] : raw).Trim().ToLowerInvariant();
            ordered = ordered is null
                ? ApplyFirst(query, field, desc)
                : ApplyThen(ordered, field, desc);
        }

        return ordered ?? query.OrderBy(s => s.SubjectId);
    }

    private static IOrderedQueryable<Subject> ApplyFirst(IQueryable<Subject> query, string field, bool desc)
        => field switch
        {
            "subjectid" => desc ? query.OrderByDescending(s => s.SubjectId) : query.OrderBy(s => s.SubjectId),
            "subjectcode" => desc ? query.OrderByDescending(s => s.SubjectCode) : query.OrderBy(s => s.SubjectCode),
            "subjectname" => desc ? query.OrderByDescending(s => s.SubjectName) : query.OrderBy(s => s.SubjectName),
            "credit" => desc ? query.OrderByDescending(s => s.Credit) : query.OrderBy(s => s.Credit),
            _ => query.OrderBy(s => s.SubjectId)
        };

    private static IOrderedQueryable<Subject> ApplyThen(IOrderedQueryable<Subject> ordered, string field, bool desc)
        => field switch
        {
            "subjectid" => desc ? ordered.ThenByDescending(s => s.SubjectId) : ordered.ThenBy(s => s.SubjectId),
            "subjectcode" => desc ? ordered.ThenByDescending(s => s.SubjectCode) : ordered.ThenBy(s => s.SubjectCode),
            "subjectname" => desc ? ordered.ThenByDescending(s => s.SubjectName) : ordered.ThenBy(s => s.SubjectName),
            "credit" => desc ? ordered.ThenByDescending(s => s.Credit) : ordered.ThenBy(s => s.Credit),
            _ => ordered.ThenBy(s => s.SubjectId)
        };
}
