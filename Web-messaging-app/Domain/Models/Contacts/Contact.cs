using Web_messaging_app.Domain.Models.UserModule;

namespace Web_messaging_app.Domain.Models.Contacts;
public class Contact
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public bool IsBlocked { get; set; }
    public bool IsMuted { get; set; }
    public DateTime CreatedAt { get; set; }

    ////////////////////////////////////////////////////
    public Guid OwnerId { get; set; }
    public Guid ContactUserId { get; set; }
    public User Owner { get; set; } = null!;
    public User ContactUser { get; set; } = null!;
}
