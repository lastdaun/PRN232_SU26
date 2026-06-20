namespace PRN232.LMS.Repositories.Entities;

public class RefreshToken
{
    public int RefreshTokenId { get; set; }

    public string Token { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    public int UserId { get; set; }

    public User User { get; set; } = null!;
}
