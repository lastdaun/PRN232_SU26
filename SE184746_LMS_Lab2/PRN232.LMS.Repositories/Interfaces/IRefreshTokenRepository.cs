using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task AddAsync(RefreshToken entity, CancellationToken cancellationToken = default);
    Task RevokeUserTokensAsync(int userId, CancellationToken cancellationToken = default);
    Task DeleteAsync(RefreshToken entity);
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
}
