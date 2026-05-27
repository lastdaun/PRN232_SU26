using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Services.Sorting;

public static class CourseSorting
{
    public static IQueryable<Course> Apply(IQueryable<Course> query, IReadOnlyList<string> sortTokens)
    {
        if (sortTokens.Count == 0)
        {
            return query.OrderBy(c => c.CourseId);
        }

        IOrderedQueryable<Course>? ordered = null;

        foreach (var raw in sortTokens)
        {
            var desc = raw.StartsWith('-');
            var field = (desc ? raw[1..] : raw).Trim().ToLowerInvariant();
            ordered = ordered is null
                ? ApplyFirst(query, field, desc)
                : ApplyThen(ordered, field, desc);
        }

        return ordered ?? query.OrderBy(c => c.CourseId);
    }

    private static IOrderedQueryable<Course> ApplyFirst(IQueryable<Course> query, string field, bool desc)
        => field switch
        {
            "courseid" => desc ? query.OrderByDescending(c => c.CourseId) : query.OrderBy(c => c.CourseId),
            "coursename" => desc ? query.OrderByDescending(c => c.CourseName) : query.OrderBy(c => c.CourseName),
            "semesterid" => desc ? query.OrderByDescending(c => c.SemesterId) : query.OrderBy(c => c.SemesterId),
            "subjectid" => desc ? query.OrderByDescending(c => c.SubjectId) : query.OrderBy(c => c.SubjectId),
            _ => query.OrderBy(c => c.CourseId)
        };

    private static IOrderedQueryable<Course> ApplyThen(IOrderedQueryable<Course> ordered, string field, bool desc)
        => field switch
        {
            "courseid" => desc ? ordered.ThenByDescending(c => c.CourseId) : ordered.ThenBy(c => c.CourseId),
            "coursename" => desc ? ordered.ThenByDescending(c => c.CourseName) : ordered.ThenBy(c => c.CourseName),
            "semesterid" => desc ? ordered.ThenByDescending(c => c.SemesterId) : ordered.ThenBy(c => c.SemesterId),
            "subjectid" => desc ? ordered.ThenByDescending(c => c.SubjectId) : ordered.ThenBy(c => c.SubjectId),
            _ => ordered.ThenBy(c => c.CourseId)
        };
}
