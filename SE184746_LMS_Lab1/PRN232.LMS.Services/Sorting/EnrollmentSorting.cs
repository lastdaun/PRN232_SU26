using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Services.Sorting;

public static class EnrollmentSorting
{
    public static IQueryable<Enrollment> Apply(IQueryable<Enrollment> query, IReadOnlyList<string> sortTokens)
    {
        if (sortTokens.Count == 0)
        {
            return query.OrderBy(e => e.EnrollmentId);
        }

        IOrderedQueryable<Enrollment>? ordered = null;

        foreach (var raw in sortTokens)
        {
            var desc = raw.StartsWith('-');
            var field = (desc ? raw[1..] : raw).Trim().ToLowerInvariant();
            ordered = ordered is null
                ? ApplyFirst(query, field, desc)
                : ApplyThen(ordered, field, desc);
        }

        return ordered ?? query.OrderBy(e => e.EnrollmentId);
    }

    private static IOrderedQueryable<Enrollment> ApplyFirst(IQueryable<Enrollment> query, string field, bool desc)
        => field switch
        {
            "enrollmentid" => desc ? query.OrderByDescending(e => e.EnrollmentId) : query.OrderBy(e => e.EnrollmentId),
            "studentid" => desc ? query.OrderByDescending(e => e.StudentId) : query.OrderBy(e => e.StudentId),
            "courseid" => desc ? query.OrderByDescending(e => e.CourseId) : query.OrderBy(e => e.CourseId),
            "enrolldate" => desc ? query.OrderByDescending(e => e.EnrollDate) : query.OrderBy(e => e.EnrollDate),
            "status" => desc ? query.OrderByDescending(e => e.Status) : query.OrderBy(e => e.Status),
            _ => query.OrderBy(e => e.EnrollmentId)
        };

    private static IOrderedQueryable<Enrollment> ApplyThen(IOrderedQueryable<Enrollment> ordered, string field, bool desc)
        => field switch
        {
            "enrollmentid" => desc ? ordered.ThenByDescending(e => e.EnrollmentId) : ordered.ThenBy(e => e.EnrollmentId),
            "studentid" => desc ? ordered.ThenByDescending(e => e.StudentId) : ordered.ThenBy(e => e.StudentId),
            "courseid" => desc ? ordered.ThenByDescending(e => e.CourseId) : ordered.ThenBy(e => e.CourseId),
            "enrolldate" => desc ? ordered.ThenByDescending(e => e.EnrollDate) : ordered.ThenBy(e => e.EnrollDate),
            "status" => desc ? ordered.ThenByDescending(e => e.Status) : ordered.ThenBy(e => e.Status),
            _ => ordered.ThenBy(e => e.EnrollmentId)
        };
}
