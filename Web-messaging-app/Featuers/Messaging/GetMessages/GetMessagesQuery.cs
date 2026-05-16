using MediatR;

namespace Web_messaging_app.Featuers.Messaging.GetMessages;

public record GetMessagesQuery(
    Guid ConversationId,
    int Page,
    int PageSize
) : IRequest<GetMessagesResponse>;

public record GetMessagesResponse(
    List<MessageDto> Messages,
    int Page,
    int PageSize,
    bool HasMore
);

public record MessageDto(
    string MessageId,
    Guid SenderId,
    string? Text,
    bool IsDeleted,
    DateTime CreatedAt
);
