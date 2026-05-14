using Web_messaging_app.Domain.Models.Conversations;

namespace Web_messaging_app.Featuers.Conversations.GetOrCreateDirectConversation;

public record ConversationResponse(
    Guid ConversationId,
    ConversationType Type,
    string? Title,
    string? AvatarUrl,
    DateTime CreatedAt
    );
