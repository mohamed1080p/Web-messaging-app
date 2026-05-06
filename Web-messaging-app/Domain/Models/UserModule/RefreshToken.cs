using System.ComponentModel.DataAnnotations.Schema;

namespace Web_messaging_app.Domain.Models.UserModule;
public class RefreshToken
{
    public Guid Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public string? ReplacedByToken { get; set; }
    public DateTime? RevokedAt { get; set; }
    [NotMapped]
    public bool IsActive => RevokedAt == null && DateTime.UtcNow < ExpiresAt;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }

    ///////////////////////////////////////////////////
    public User User { get; set; } = null!;
    public Guid UserId { get; set; }

}
