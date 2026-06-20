using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Implementations;

public class UserRepository : IUserRepository
{
    private readonly LmsDbContext _context;

    public UserRepository(LmsDbContext context) => _context = context;

    public async Task<(IEnumerable<User> Items, int Total)> GetAllAsync(
        string? search, string? sort, int page, int size,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(u => u.Username.Contains(search) || u.Role.Contains(search));

        query = ApplySort(query, sort);

        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((page - 1) * size).Take(size).ToListAsync(cancellationToken);
        return (items, total);
    }

    public Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
        => _context.Users.FirstOrDefaultAsync(u => u.Username == username, cancellationToken);

    public Task<User?> GetByIdAsync(int userId, CancellationToken cancellationToken = default)
        => _context.Users.FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);

    public Task<bool> UsernameExistsAsync(string username, int? excludeUserId = null,
        CancellationToken cancellationToken = default)
        => _context.Users.AnyAsync(
            u => u.Username == username && (excludeUserId == null || u.UserId != excludeUserId),
            cancellationToken);

    public async Task AddAsync(User entity, CancellationToken cancellationToken = default)
        => await _context.Users.AddAsync(entity, cancellationToken);

    public Task UpdateAsync(User entity)
    {
        _context.Users.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(User entity)
    {
        _context.Users.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken) > 0;

    private static IQueryable<User> ApplySort(IQueryable<User> query, string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort)) return query.OrderBy(u => u.UserId);
        var parts = sort.Split(',', StringSplitOptions.RemoveEmptyEntries);
        IOrderedQueryable<User>? ordered = null;
        foreach (var part in parts)
        {
            var token = part.Trim();
            var desc = token.StartsWith('-');
            var field = token.TrimStart('-').ToLower();
            ordered = (field, desc, ordered) switch
            {
                ("username", false, null) => query.OrderBy(u => u.Username),
                ("username", true, null) => query.OrderByDescending(u => u.Username),
                ("role", false, null) => query.OrderBy(u => u.Role),
                ("role", true, null) => query.OrderByDescending(u => u.Role),
                ("username", false, _) => ordered!.ThenBy(u => u.Username),
                ("username", true, _) => ordered!.ThenByDescending(u => u.Username),
                ("role", false, _) => ordered!.ThenBy(u => u.Role),
                ("role", true, _) => ordered!.ThenByDescending(u => u.Role),
                _ => ordered ?? query.OrderBy(u => u.UserId)
            };
        }
        return ordered ?? query.OrderBy(u => u.UserId);
    }
}
