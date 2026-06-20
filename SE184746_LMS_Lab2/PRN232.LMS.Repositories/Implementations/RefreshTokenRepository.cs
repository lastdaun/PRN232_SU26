using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Implementations;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly LmsDbContext _context;

    public RefreshTokenRepository(LmsDbContext context) => _context = context;

    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
        => await _context.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == token, cancellationToken);

    public async Task AddAsync(RefreshToken entity, CancellationToken cancellationToken = default)
        => await _context.RefreshTokens.AddAsync(entity, cancellationToken);

    public async Task RevokeUserTokensAsync(int userId, CancellationToken cancellationToken = default)
    {
        var tokens = await _context.RefreshTokens
            .Where(r => r.UserId == userId)
            .ToListAsync(cancellationToken);
        _context.RefreshTokens.RemoveRange(tokens);
    }

    public Task DeleteAsync(RefreshToken entity)
    {
        _context.RefreshTokens.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken) > 0;
}
