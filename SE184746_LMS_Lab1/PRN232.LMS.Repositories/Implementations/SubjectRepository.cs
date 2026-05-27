using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Helpers;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Implementations;

public sealed class SubjectRepository : ISubjectRepository
{
    private readonly LmsDbContext _context;

    public SubjectRepository(LmsDbContext context)
    {
        _context = context;
    }

    public IQueryable<Subject> QuerySubjects(IEnumerable<string> expand)
    {
        var query = _context.Subjects.AsNoTracking().AsQueryable();
        var expandSet = ExpandTokenParser.ToSet(expand);

        if (expandSet.Contains("courses"))
        {
            query = query.Include(s => s.Courses);
        }

        if (expandSet.Contains("courses.semester"))
        {
            query = query
                .Include(s => s.Courses)
                .ThenInclude(c => c.Semester);
        }

        return query;
    }

    public Task<Subject?> GetByIdAsync(int id, IEnumerable<string> expand, CancellationToken cancellationToken = default)
    {
        return QuerySubjects(expand).FirstOrDefaultAsync(s => s.SubjectId == id, cancellationToken);
    }

    public Task<bool> SubjectCodeExistsAsync(string subjectCode, int? excludeSubjectId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Subjects.Where(s => s.SubjectCode == subjectCode);
        if (excludeSubjectId.HasValue)
        {
            query = query.Where(s => s.SubjectId != excludeSubjectId.Value);
        }

        return query.AnyAsync(cancellationToken);
    }

    public async Task<Subject> AddAsync(Subject subject, CancellationToken cancellationToken = default)
    {
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync(cancellationToken);
        return subject;
    }

    public Task<Subject?> FindTrackedByIdAsync(int id, CancellationToken cancellationToken = default)
        => _context.Subjects.FirstOrDefaultAsync(s => s.SubjectId == id, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);

    public async Task<bool> DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var subject = await FindTrackedByIdAsync(id, cancellationToken);
        if (subject is null)
        {
            return false;
        }

        _context.Subjects.Remove(subject);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
