using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Helpers;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Implementations;

public sealed class SemesterRepository : ISemesterRepository
{
    private readonly LmsDbContext _context;

    public SemesterRepository(LmsDbContext context)
    {
        _context = context;
    }

    public IQueryable<Semester> QuerySemesters(IEnumerable<string> expand)
    {
        var query = _context.Semesters.AsNoTracking().AsQueryable();
        var expandSet = ExpandTokenParser.ToSet(expand);

        if (expandSet.Contains("courses"))
        {
            query = query.Include(s => s.Courses);
        }

        if (expandSet.Contains("courses.subject"))
        {
            query = query
                .Include(s => s.Courses)
                .ThenInclude(c => c.Subject);
        }

        return query;
    }

    public Task<Semester?> GetByIdAsync(int id, IEnumerable<string> expand, CancellationToken cancellationToken = default)
    {
        return QuerySemesters(expand).FirstOrDefaultAsync(s => s.SemesterId == id, cancellationToken);
    }

    public async Task<Semester> AddAsync(Semester semester, CancellationToken cancellationToken = default)
    {
        _context.Semesters.Add(semester);
        await _context.SaveChangesAsync(cancellationToken);
        return semester;
    }

    public Task<Semester?> FindTrackedByIdAsync(int id, CancellationToken cancellationToken = default)
        => _context.Semesters.FirstOrDefaultAsync(s => s.SemesterId == id, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);

    public async Task<bool> DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var semester = await FindTrackedByIdAsync(id, cancellationToken);
        if (semester is null)
        {
            return false;
        }

        _context.Semesters.Remove(semester);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
