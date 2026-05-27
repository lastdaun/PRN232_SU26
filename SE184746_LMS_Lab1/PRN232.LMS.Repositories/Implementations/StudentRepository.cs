using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Helpers;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Implementations;

public sealed class StudentRepository : IStudentRepository
{
    private readonly LmsDbContext _context;

    public StudentRepository(LmsDbContext context)
    {
        _context = context;
    }

    public IQueryable<Student> QueryStudents(IEnumerable<string> expand)
    {
        var query = _context.Students.AsNoTracking().AsQueryable();

        var expandSet = ExpandTokenParser.ToSet(expand);
        if (expandSet.Contains("enrollments"))
        {
            query = query.Include(s => s.Enrollments);
        }

        if (expandSet.Contains("enrollments.course"))
        {
            query = query
                .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course);
        }

        if (expandSet.Contains("enrollments.course.semester"))
        {
            query = query
                .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                .ThenInclude(c => c.Semester);
        }

        if (expandSet.Contains("enrollments.course.subject"))
        {
            query = query
                .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                .ThenInclude(c => c.Subject);
        }

        return query;
    }

    public Task<Student?> GetByIdAsync(int id, IEnumerable<string> expand, CancellationToken cancellationToken = default)
    {
        return QueryStudents(expand).FirstOrDefaultAsync(s => s.StudentId == id, cancellationToken);
    }

    public Task<bool> EmailExistsAsync(string email, int? excludeStudentId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Students.Where(s => s.Email == email);
        if (excludeStudentId.HasValue)
        {
            query = query.Where(s => s.StudentId != excludeStudentId.Value);
        }

        return query.AnyAsync(cancellationToken);
    }

    public async Task<Student> AddAsync(Student student, CancellationToken cancellationToken = default)
    {
        _context.Students.Add(student);
        await _context.SaveChangesAsync(cancellationToken);
        return student;
    }

    public Task<Student?> FindTrackedByIdAsync(int id, CancellationToken cancellationToken = default)
        => _context.Students.FirstOrDefaultAsync(s => s.StudentId == id, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);

    public async Task<bool> DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var student = await FindTrackedByIdAsync(id, cancellationToken);
        if (student is null)
        {
            return false;
        }

        _context.Students.Remove(student);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

