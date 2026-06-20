using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Implementations;

public class SubjectRepository : ISubjectRepository
{
    private readonly LmsDbContext _context;
    public SubjectRepository(LmsDbContext context) => _context = context;

    public async Task<(IEnumerable<Subject> Items, int Total)> GetAllAsync(
        string? search, string? sort, int page, int size, bool includeCourses,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Subjects.AsQueryable();

        if (includeCourses) query = query.Include(s => s.Courses);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(s => s.SubjectName.Contains(search) || s.SubjectCode.Contains(search));

        query = ApplySort(query, sort);

        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((page - 1) * size).Take(size).ToListAsync(cancellationToken);
        return (items, total);
    }

    public async Task<Subject?> GetByIdAsync(int id, bool includeCourses = false,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Subjects.AsQueryable();
        if (includeCourses) query = query.Include(s => s.Courses);
        return await query.FirstOrDefaultAsync(s => s.SubjectId == id, cancellationToken);
    }

    public async Task AddAsync(Subject entity, CancellationToken cancellationToken = default)
        => await _context.Subjects.AddAsync(entity, cancellationToken);

    public Task UpdateAsync(Subject entity)
    {
        _context.Subjects.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Subject entity)
    {
        _context.Subjects.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken) > 0;

    private static IQueryable<Subject> ApplySort(IQueryable<Subject> query, string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort))
            return query.OrderBy(s => s.SubjectId);

        IOrderedQueryable<Subject>? ordered = null;
        foreach (var field in sort.Split(','))
        {
            bool desc = field.Trim().StartsWith('-');
            var name = field.Trim().TrimStart('-').ToLower();
            ordered = (name, desc, ordered) switch
            {
                ("subjectname", false, null) => query.OrderBy(s => s.SubjectName),
                ("subjectname", true, null) => query.OrderByDescending(s => s.SubjectName),
                ("subjectcode", false, null) => query.OrderBy(s => s.SubjectCode),
                ("subjectcode", true, null) => query.OrderByDescending(s => s.SubjectCode),
                ("credit", false, null) => query.OrderBy(s => s.Credit),
                ("credit", true, null) => query.OrderByDescending(s => s.Credit),
                ("subjectname", false, _) => ordered!.ThenBy(s => s.SubjectName),
                ("subjectname", true, _) => ordered!.ThenByDescending(s => s.SubjectName),
                _ => ordered ?? query.OrderBy(s => s.SubjectId)
            };
        }
        return ordered ?? query.OrderBy(s => s.SubjectId);
    }
}
