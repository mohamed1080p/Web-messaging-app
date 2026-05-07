using Web_messaging_app.Domain.Models.UserModule;

namespace Web_messaging_app.Domain.Models.Contacts;
public class UserBlock
{
    public Guid Id { get; set; }
    public Guid BlockerUserId { get; set; }
    public Guid BlockedUserId { get; set; }
    public DateTime CreatedAt { get; set; }


    //////////////////////////////////////////////////////////////
    public User Blocker { get; set; } = null!;
    public User Blocked { get; set; } = null!;
}
