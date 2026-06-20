using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.Exceptions;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Business;

namespace PRN232.LMS.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IConfiguration _configuration;

    public AuthService(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _configuration = configuration;
    }

    public async Task<TokenBusiness> LoginAsync(string username, string password,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByUsernameAsync(username, cancellationToken);

        if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            throw new InvalidCredentialsException();

        // Revoke old refresh tokens for this user
        await _refreshTokenRepository.RevokeUserTokensAsync(user.UserId, cancellationToken);
        await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

        return await GenerateTokenPairAsync(user, cancellationToken);
    }

    public async Task<TokenBusiness> RefreshTokenAsync(string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var stored = await _refreshTokenRepository.GetByTokenAsync(refreshToken, cancellationToken);

        if (stored is null)
            throw new InvalidCredentialsException("Invalid refresh token.");

        if (stored.IsExpired)
        {
            await _refreshTokenRepository.DeleteAsync(stored);
            await _refreshTokenRepository.SaveChangesAsync(cancellationToken);
            throw new InvalidCredentialsException("Refresh token has expired.");
        }

        var user = stored.User;

        // Rotate: delete old token, issue new pair
        await _refreshTokenRepository.DeleteAsync(stored);
        await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

        return await GenerateTokenPairAsync(user, cancellationToken);
    }

    private async Task<TokenBusiness> GenerateTokenPairAsync(User user, CancellationToken cancellationToken)
    {
        var accessToken = GenerateAccessToken(user.UserId, user.Username, user.Role);
        var rawRefreshToken = GenerateRawRefreshToken();
        var expiresIn = int.Parse(_configuration["Jwt:ExpiresInSeconds"] ?? "3600");

        var refreshTokenEntity = new RefreshToken
        {
            Token = rawRefreshToken,
            UserId = user.UserId,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        await _refreshTokenRepository.AddAsync(refreshTokenEntity, cancellationToken);
        await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

        return new TokenBusiness
        {
            AccessToken = accessToken,
            RefreshToken = rawRefreshToken,
            ExpiresIn = expiresIn
        };
    }

    private string GenerateAccessToken(int userId, string username, string role)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT secret not configured.")));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresIn = int.Parse(_configuration["Jwt:ExpiresInSeconds"] ?? "3600");

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, username),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddSeconds(expiresIn),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRawRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
}
