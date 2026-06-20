using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Implementations;

public class StudentRepository : IStudentRepository
{
    private readonly LmsDbContext _context;
    public StudentRepository(LmsDbContext context) => _context = context;

    public async Task<(IEnumerable<Student> Items, int Total)> GetAllAsync(
        string? search, string? sort, int page, int size, bool includeEnrollments,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Students.AsQueryable();

        if (includeEnrollments)
            query = query.Include(s => s.Enrollments).ThenInclude(e => e.Course);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(s => s.FullName.Contains(search) || s.Email.Contains(search));

        query = ApplySort(query, sort);

        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((page - 1) * size).Take(size).ToListAsync(cancellationToken);
        return (items, total);
    }

    public async Task<Student?> GetByIdAsync(int id, bool includeEnrollments = false,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Students.AsQueryable();
        if (includeEnrollments)
            query = query.Include(s => s.Enrollments).ThenInclude(e => e.Course);
        return await query.FirstOrDefaultAsync(s => s.StudentId == id, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, int? excludeStudentId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Students.Where(s => s.Email == email);
        if (excludeStudentId.HasValue)
            query = query.Where(s => s.StudentId != excludeStudentId.Value);
        return await query.AnyAsync(cancellationToken);
    }

    public async Task AddAsync(Student entity, CancellationToken cancellationToken = default)
        => await _context.Students.AddAsync(entity, cancellationToken);

    public Task UpdateAsync(Student entity)
    {
        _context.Students.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Student entity)
    {
        _context.Students.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken) > 0;

    private static IQueryable<Student> ApplySort(IQueryable<Student> query, string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort))
            return query.OrderBy(s => s.StudentId);

        IOrderedQueryable<Student>? ordered = null;
        foreach (var field in sort.Split(','))
        {
            bool desc = field.Trim().StartsWith('-');
            var name = field.Trim().TrimStart('-').ToLower();
            ordered = (name, desc, ordered) switch
            {
                ("fullname", false, null) => query.OrderBy(s => s.FullName),
                ("fullname", true, null) => query.OrderByDescending(s => s.FullName),
                ("email", false, null) => query.OrderBy(s => s.Email),
                ("email", true, null) => query.OrderByDescending(s => s.Email),
                ("dateofbirth", false, null) => query.OrderBy(s => s.DateOfBirth),
                ("dateofbirth", true, null) => query.OrderByDescending(s => s.DateOfBirth),
                ("fullname", false, _) => ordered!.ThenBy(s => s.FullName),
                ("fullname", true, _) => ordered!.ThenByDescending(s => s.FullName),
                ("email", false, _) => ordered!.ThenBy(s => s.Email),
                ("email", true, _) => ordered!.ThenByDescending(s => s.Email),
                ("dateofbirth", false, _) => ordered!.ThenBy(s => s.DateOfBirth),
                ("dateofbirth", true, _) => ordered!.ThenByDescending(s => s.DateOfBirth),
                _ => ordered ?? query.OrderBy(s => s.StudentId)
            };
        }
        return ordered ?? query.OrderBy(s => s.StudentId);
    }
}
