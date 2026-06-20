using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Implementations;

public class CourseRepository : ICourseRepository
{
    private readonly LmsDbContext _context;
    public CourseRepository(LmsDbContext context) => _context = context;

    public async Task<(IEnumerable<Course> Items, int Total)> GetAllAsync(
        string? search, string? sort, int page, int size,
        bool includeSemester, bool includeSubject, bool includeEnrollments,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Courses.AsQueryable();

        if (includeSemester) query = query.Include(c => c.Semester);
        if (includeSubject) query = query.Include(c => c.Subject);
        if (includeEnrollments) query = query.Include(c => c.Enrollments).ThenInclude(e => e.Student);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(c => c.CourseName.Contains(search));

        query = ApplySort(query, sort);

        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((page - 1) * size).Take(size).ToListAsync(cancellationToken);
        return (items, total);
    }

    public async Task<Course?> GetByIdAsync(int id,
        bool includeSemester = false, bool includeSubject = false, bool includeEnrollments = false,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Courses.AsQueryable();
        if (includeSemester) query = query.Include(c => c.Semester);
        if (includeSubject) query = query.Include(c => c.Subject);
        if (includeEnrollments) query = query.Include(c => c.Enrollments).ThenInclude(e => e.Student);
        return await query.FirstOrDefaultAsync(c => c.CourseId == id, cancellationToken);
    }

    public async Task AddAsync(Course entity, CancellationToken cancellationToken = default)
        => await _context.Courses.AddAsync(entity, cancellationToken);

    public Task UpdateAsync(Course entity)
    {
        _context.Courses.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Course entity)
    {
        _context.Courses.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken) > 0;

    private static IQueryable<Course> ApplySort(IQueryable<Course> query, string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort))
            return query.OrderBy(c => c.CourseId);

        IOrderedQueryable<Course>? ordered = null;
        foreach (var field in sort.Split(','))
        {
            bool desc = field.Trim().StartsWith('-');
            var name = field.Trim().TrimStart('-').ToLower();
            ordered = (name, desc, ordered) switch
            {
                ("coursename", false, null) => query.OrderBy(c => c.CourseName),
                ("coursename", true, null) => query.OrderByDescending(c => c.CourseName),
                ("coursename", false, _) => ordered!.ThenBy(c => c.CourseName),
                ("coursename", true, _) => ordered!.ThenByDescending(c => c.CourseName),
                _ => ordered ?? query.OrderBy(c => c.CourseId)
            };
        }
        return ordered ?? query.OrderBy(c => c.CourseId);
    }
}
