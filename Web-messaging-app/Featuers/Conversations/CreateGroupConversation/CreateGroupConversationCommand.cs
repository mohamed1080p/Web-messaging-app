using MediatR;
using Web_messaging_app.Featuers.Conversations.GetOrCreateDirectConversation;

namespace Web_messaging_app.Featuers.Conversations.CreateGroupConversation;

public record CreateGroupConversationCommand(
    string Title,
    string? AvatarUrl,
    string? Description,
    List<Guid> ParticipantUserIds) : IRequest<ConversationResponse>;
