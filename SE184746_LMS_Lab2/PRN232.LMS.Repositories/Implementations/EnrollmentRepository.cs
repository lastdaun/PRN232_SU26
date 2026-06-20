using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Implementations;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly LmsDbContext _context;
    public EnrollmentRepository(LmsDbContext context) => _context = context;

    public async Task<(IEnumerable<Enrollment> Items, int Total)> GetAllAsync(
        string? search, string? sort, int page, int size,
        bool includeStudent, bool includeCourse,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(includeStudent, includeCourse);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(e => e.Status.Contains(search)
                || (includeStudent && e.Student.FullName.Contains(search)));

        query = ApplySort(query, sort);

        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((page - 1) * size).Take(size).ToListAsync(cancellationToken);
        return (items, total);
    }

    public async Task<(IEnumerable<Enrollment> Items, int Total)> GetAllByCourseIdAsync(
        int courseId, string? search, string? sort, int page, int size,
        bool includeStudent, bool includeCourse,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(includeStudent, includeCourse)
            .Where(e => e.CourseId == courseId);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(e => e.Status.Contains(search)
                || (includeStudent && e.Student.FullName.Contains(search)));

        query = ApplySort(query, sort);

        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((page - 1) * size).Take(size).ToListAsync(cancellationToken);
        return (items, total);
    }

    public async Task<Enrollment?> GetByIdAsync(int id,
        bool includeStudent = false, bool includeCourse = false,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(includeStudent, includeCourse);
        return await query.FirstOrDefaultAsync(e => e.EnrollmentId == id, cancellationToken);
    }

    public async Task<bool> ExistsByStudentAndCourseAsync(int studentId, int courseId, int? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Enrollments
            .Where(e => e.StudentId == studentId && e.CourseId == courseId);
        if (excludeId.HasValue)
            query = query.Where(e => e.EnrollmentId != excludeId.Value);
        return await query.AnyAsync(cancellationToken);
    }

    public async Task AddAsync(Enrollment entity, CancellationToken cancellationToken = default)
        => await _context.Enrollments.AddAsync(entity, cancellationToken);

    public Task UpdateAsync(Enrollment entity)
    {
        _context.Enrollments.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Enrollment entity)
    {
        _context.Enrollments.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken) > 0;

    private IQueryable<Enrollment> BuildQuery(bool includeStudent, bool includeCourse)
    {
        var query = _context.Enrollments.AsQueryable();
        if (includeStudent) query = query.Include(e => e.Student);
        if (includeCourse) query = query.Include(e => e.Course).ThenInclude(c => c.Semester);
        return query;
    }

    private static IQueryable<Enrollment> ApplySort(IQueryable<Enrollment> query, string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort))
            return query.OrderBy(e => e.EnrollmentId);

        IOrderedQueryable<Enrollment>? ordered = null;
        foreach (var field in sort.Split(','))
        {
            bool desc = field.Trim().StartsWith('-');
            var name = field.Trim().TrimStart('-').ToLower();
            ordered = (name, desc, ordered) switch
            {
                ("enrolldate", false, null) => query.OrderBy(e => e.EnrollDate),
                ("enrolldate", true, null) => query.OrderByDescending(e => e.EnrollDate),
                ("status", false, null) => query.OrderBy(e => e.Status),
                ("status", true, null) => query.OrderByDescending(e => e.Status),
                ("enrolldate", false, _) => ordered!.ThenBy(e => e.EnrollDate),
                ("enrolldate", true, _) => ordered!.ThenByDescending(e => e.EnrollDate),
                ("status", false, _) => ordered!.ThenBy(e => e.Status),
                ("status", true, _) => ordered!.ThenByDescending(e => e.Status),
                _ => ordered ?? query.OrderBy(e => e.EnrollmentId)
            };
        }
        return ordered ?? query.OrderBy(e => e.EnrollmentId);
    }
}
