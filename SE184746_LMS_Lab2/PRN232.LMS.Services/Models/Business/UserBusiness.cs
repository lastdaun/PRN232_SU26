namespace PRN232.LMS.Services.Models.Business;

public class UserBusiness
{
    public int UserId { get; set; }
    public string Username { get; set; } = null!;
    public string? Password { get; set; }
    public string PasswordHash { get; set; } = null!;
    public string Role { get; set; } = null!;
}
