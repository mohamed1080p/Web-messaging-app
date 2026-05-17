using MediatR;

namespace Web_messaging_app.Featuers.Messaging.DeleteMessage;

public record DeleteMessageCommand(string MessageId, Guid ConversationId) : IRequest;
