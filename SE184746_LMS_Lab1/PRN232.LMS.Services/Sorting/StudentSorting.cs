using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Services.Sorting;

public static class StudentSorting
{
    public static IQueryable<Student> Apply(IQueryable<Student> query, IReadOnlyList<string> sortTokens)
    {
        if (sortTokens.Count == 0)
        {
            return query.OrderBy(s => s.StudentId);
        }

        IOrderedQueryable<Student>? ordered = null;

        foreach (var raw in sortTokens)
        {
            var desc = raw.StartsWith('-');
            var field = desc ? raw[1..] : raw;
            field = field.Trim().ToLowerInvariant();

            if (ordered is null)
            {
                ordered = ApplyFirst(query, field, desc);
            }
            else
            {
                ordered = ApplyThen(ordered, field, desc);
            }
        }

        return ordered ?? query.OrderBy(s => s.StudentId);
    }

    private static IOrderedQueryable<Student> ApplyFirst(IQueryable<Student> query, string field, bool desc)
        => field switch
        {
            "studentid" => desc ? query.OrderByDescending(s => s.StudentId) : query.OrderBy(s => s.StudentId),
            "fullname" => desc ? query.OrderByDescending(s => s.FullName) : query.OrderBy(s => s.FullName),
            "email" => desc ? query.OrderByDescending(s => s.Email) : query.OrderBy(s => s.Email),
            "dateofbirth" => desc ? query.OrderByDescending(s => s.DateOfBirth) : query.OrderBy(s => s.DateOfBirth),
            _ => query.OrderBy(s => s.StudentId)
        };

    private static IOrderedQueryable<Student> ApplyThen(IOrderedQueryable<Student> ordered, string field, bool desc)
        => field switch
        {
            "studentid" => desc ? ordered.ThenByDescending(s => s.StudentId) : ordered.ThenBy(s => s.StudentId),
            "fullname" => desc ? ordered.ThenByDescending(s => s.FullName) : ordered.ThenBy(s => s.FullName),
            "email" => desc ? ordered.ThenByDescending(s => s.Email) : ordered.ThenBy(s => s.Email),
            "dateofbirth" => desc ? ordered.ThenByDescending(s => s.DateOfBirth) : ordered.ThenBy(s => s.DateOfBirth),
            _ => ordered.ThenBy(s => s.StudentId)
        };
}
