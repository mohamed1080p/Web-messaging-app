using System.ComponentModel.DataAnnotations.Schema;

namespace Web_messaging_app.Domain.Models.UserModule;
public class RefreshToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;

    [NotMapped]
    public bool IsActive => DateTime.UtcNow < ExpiresAt;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }

    ///////////////////////////////////////////////////
    public User User { get; set; } = null!;
   

}
