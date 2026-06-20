using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Implementations;

public class SemesterRepository : ISemesterRepository
{
    private readonly LmsDbContext _context;
    public SemesterRepository(LmsDbContext context) => _context = context;

    public async Task<(IEnumerable<Semester> Items, int Total)> GetAllAsync(
        string? search, string? sort, int page, int size, bool includeCourses,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Semesters.AsQueryable();

        if (includeCourses) query = query.Include(s => s.Courses);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(s => s.SemesterName.Contains(search));

        query = ApplySort(query, sort);

        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((page - 1) * size).Take(size).ToListAsync(cancellationToken);
        return (items, total);
    }

    public async Task<Semester?> GetByIdAsync(int id, bool includeCourses = false,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Semesters.AsQueryable();
        if (includeCourses) query = query.Include(s => s.Courses);
        return await query.FirstOrDefaultAsync(s => s.SemesterId == id, cancellationToken);
    }

    public async Task AddAsync(Semester entity, CancellationToken cancellationToken = default)
        => await _context.Semesters.AddAsync(entity, cancellationToken);

    public Task UpdateAsync(Semester entity)
    {
        _context.Semesters.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Semester entity)
    {
        _context.Semesters.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken) > 0;

    private static IQueryable<Semester> ApplySort(IQueryable<Semester> query, string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort))
            return query.OrderBy(s => s.SemesterId);

        IOrderedQueryable<Semester>? ordered = null;
        foreach (var field in sort.Split(','))
        {
            bool desc = field.Trim().StartsWith('-');
            var name = field.Trim().TrimStart('-').ToLower();
            ordered = (name, desc, ordered) switch
            {
                ("semestername", false, null) => query.OrderBy(s => s.SemesterName),
                ("semestername", true, null) => query.OrderByDescending(s => s.SemesterName),
                ("startdate", false, null) => query.OrderBy(s => s.StartDate),
                ("startdate", true, null) => query.OrderByDescending(s => s.StartDate),
                ("enddate", false, null) => query.OrderBy(s => s.EndDate),
                ("enddate", true, null) => query.OrderByDescending(s => s.EndDate),
                ("semestername", false, _) => ordered!.ThenBy(s => s.SemesterName),
                ("semestername", true, _) => ordered!.ThenByDescending(s => s.SemesterName),
                _ => ordered ?? query.OrderBy(s => s.SemesterId)
            };
        }
        return ordered ?? query.OrderBy(s => s.SemesterId);
    }
}
