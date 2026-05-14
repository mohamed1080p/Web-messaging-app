using MediatR;
namespace Web_messaging_app.Featuers.Conversations.GetOrCreateDirectConversation;

public record GetOrCreateDirectConversationCommand(Guid ContactUserId) :
    IRequest<GetOrCreateDirectConversationResponse>;
