using MediatR;

namespace Web_messaging_app.Featuers.Messaging.SendMessage;

public record SendMessageCommand(
    Guid ConversationId,
    string? Text
    ) : IRequest<SendMessageResponse>;



public record SendMessageResponse(
    string MessageId,
    Guid ConversationId,
    Guid SenderId,
    string? Text,
    DateTime CreatedAt
    );



