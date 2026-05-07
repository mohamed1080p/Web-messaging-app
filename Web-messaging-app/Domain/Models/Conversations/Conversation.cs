using Web_messaging_app.Domain.Models.UserModule;
namespace Web_messaging_app.Domain.Models.Conversations;
public class Conversation
{
    public Guid Id { get; set; }
    public ConversationType Type { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? AvatarUrl { get; set; }
    public Guid CreatedByUserId { get; set; }
    public DateTime? LastMessageAt { get; set; }
    public string? LastMessagePreview { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    //////////////////////////////////////////////////////
    public User CreatedBy { get; set; } = null!;
    public ICollection<ConversationParticipant> Participants { get; set; } = [];
}
