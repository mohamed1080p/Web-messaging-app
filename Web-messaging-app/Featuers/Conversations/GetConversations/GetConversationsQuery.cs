using MediatR;
using Web_messaging_app.Domain.Models.Conversations;

namespace Web_messaging_app.Featuers.Conversations.GetConversations;

public record GetConversationsQuery : IRequest<List<ConversationListDto>>;

public record ConversationListDto(
    Guid ConversationId,
    ConversationType Type,
    string? Title,
    string? AvatarUrl,
    string? LastMessagePreview,
    DateTime? LastMessageAt
);