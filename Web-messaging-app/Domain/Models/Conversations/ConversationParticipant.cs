using Web_messaging_app.Domain.Models.UserModule;

namespace Web_messaging_app.Domain.Models.Conversations;
public class ConversationParticipant
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public Guid UserId { get; set; }
    public ParticipantRole Role { get; set; }
    public DateTime JoinedAt { get; set; }
    public DateTime? MutedUntil { get; set; }
    public DateTime? LastReadAt { get; set; }

    /////////////////////////////////////////////////////////////
    public Conversation Conversation { get; set; } = null!;
    public User User { get; set; } = null!;
}
