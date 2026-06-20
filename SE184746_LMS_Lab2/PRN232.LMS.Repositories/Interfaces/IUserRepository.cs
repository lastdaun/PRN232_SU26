using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Interfaces;

public interface IUserRepository
{
    Task<(IEnumerable<User> Items, int Total)> GetAllAsync(
        string? search, string? sort, int page, int size,
        CancellationToken cancellationToken = default);

    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);

    Task<User?> GetByIdAsync(int userId, CancellationToken cancellationToken = default);

    Task<bool> UsernameExistsAsync(string username, int? excludeUserId = null,
        CancellationToken cancellationToken = default);

    Task AddAsync(User entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(User entity);
    Task DeleteAsync(User entity);
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
}
