using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Helpers;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Implementations;

public sealed class EnrollmentRepository : IEnrollmentRepository
{
    private readonly LmsDbContext _context;

    public EnrollmentRepository(LmsDbContext context)
    {
        _context = context;
    }

    public IQueryable<Enrollment> QueryEnrollments(IEnumerable<string> expand)
    {
        var query = _context.Enrollments.AsNoTracking().AsQueryable();
        var expandSet = ExpandTokenParser.ToSet(expand);

        if (expandSet.Contains("student"))
        {
            query = query.Include(e => e.Student);
        }

        if (expandSet.Contains("course"))
        {
            query = query.Include(e => e.Course);
        }

        if (expandSet.Contains("course.semester"))
        {
            query = query
                .Include(e => e.Course)
                .ThenInclude(c => c.Semester);
        }

        if (expandSet.Contains("course.subject"))
        {
            query = query
                .Include(e => e.Course)
                .ThenInclude(c => c.Subject);
        }

        return query;
    }

    public Task<Enrollment?> GetByIdAsync(int id, IEnumerable<string> expand, CancellationToken cancellationToken = default)
    {
        return QueryEnrollments(expand).FirstOrDefaultAsync(e => e.EnrollmentId == id, cancellationToken);
    }

    public async Task<Enrollment> AddAsync(Enrollment enrollment, CancellationToken cancellationToken = default)
    {
        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync(cancellationToken);
        return enrollment;
    }

    public Task<Enrollment?> FindTrackedByIdAsync(int id, CancellationToken cancellationToken = default)
        => _context.Enrollments.FirstOrDefaultAsync(e => e.EnrollmentId == id, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);

    public async Task<bool> DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var enrollment = await FindTrackedByIdAsync(id, cancellationToken);
        if (enrollment is null)
        {
            return false;
        }

        _context.Enrollments.Remove(enrollment);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
