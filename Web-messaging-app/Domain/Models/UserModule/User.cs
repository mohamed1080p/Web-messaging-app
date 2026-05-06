namespace Web_messaging_app.Domain.Models.UserModule;
public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? Bio { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastSeenAt { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];

}
