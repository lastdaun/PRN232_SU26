using PRN232.LMS.Services.Models.Business;

namespace PRN232.LMS.Services.Interfaces;

public interface IAuthService
{
    Task<TokenBusiness> LoginAsync(string username, string password,
        CancellationToken cancellationToken = default);

    Task<TokenBusiness> RefreshTokenAsync(string refreshToken,
        CancellationToken cancellationToken = default);
}
