using MediatR;
using Web_messaging_app.Domain.Models.Conversations;

namespace Web_messaging_app.Featuers.Conversations.GetConversationDetails;

public record GetConversationDetailsQuery(
    Guid ConversationId
) : IRequest<ConversationDetailsDto>;

public record ConversationDetailsDto(
    Guid ConversationId,
    ConversationType Type,
    string? Title,
    string? Description,
    string? AvatarUrl,
    DateTime CreatedAt,
    List<ParticipantDto> Participants
);

public record ParticipantDto(
    Guid UserId,
    string Username,
    string? AvatarUrl,
    ParticipantRole Role,
    DateTime JoinedAt
);