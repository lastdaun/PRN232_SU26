using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Helpers;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Implementations;

public sealed class CourseRepository : ICourseRepository
{
    private readonly LmsDbContext _context;

    public CourseRepository(LmsDbContext context)
    {
        _context = context;
    }

    public IQueryable<Course> QueryCourses(IEnumerable<string> expand)
    {
        var query = _context.Courses.AsNoTracking().AsQueryable();
        var expandSet = ExpandTokenParser.ToSet(expand);

        if (expandSet.Contains("semester"))
        {
            query = query.Include(c => c.Semester);
        }

        if (expandSet.Contains("subject"))
        {
            query = query.Include(c => c.Subject);
        }

        if (expandSet.Contains("enrollments"))
        {
            query = query.Include(c => c.Enrollments);
        }

        if (expandSet.Contains("enrollments.student"))
        {
            query = query
                .Include(c => c.Enrollments)
                .ThenInclude(e => e.Student);
        }

        return query;
    }

    public Task<Course?> GetByIdAsync(int id, IEnumerable<string> expand, CancellationToken cancellationToken = default)
    {
        return QueryCourses(expand).FirstOrDefaultAsync(c => c.CourseId == id, cancellationToken);
    }

    public async Task<Course> AddAsync(Course course, CancellationToken cancellationToken = default)
    {
        _context.Courses.Add(course);
        await _context.SaveChangesAsync(cancellationToken);
        return course;
    }

    public Task<Course?> FindTrackedByIdAsync(int id, CancellationToken cancellationToken = default)
        => _context.Courses.FirstOrDefaultAsync(c => c.CourseId == id, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);

    public async Task<bool> DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var course = await FindTrackedByIdAsync(id, cancellationToken);
        if (course is null)
        {
            return false;
        }

        _context.Courses.Remove(course);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
