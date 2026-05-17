using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Web_messaging_app.Domain.Models.Messages;
using Web_messaging_app.Infrastructure.Persistence.MongoDb;
using Web_messaging_app.Infrastructure.Persistence.PostgreSql;

namespace Web_messaging_app.Featuers.Messaging.SendMessage;

public class SendMessageCommandHandler
    (AppDbContext _dbContext,
    MongoDbContext _mongoDbContext,
    IHttpContextAccessor _httpContextAccessor) : IRequestHandler<SendMessageCommand, SendMessageResponse>
{
    public async Task<SendMessageResponse> Handle(SendMessageCommand request, CancellationToken ct)
    {
        var senderId = Guid.Parse(
            _httpContextAccessor.HttpContext!.User
                .FindFirstValue(ClaimTypes.NameIdentifier)!);

        var isParticipant = await _dbContext.conversationParticipants.
            AnyAsync(a => a.ConversationId == request.ConversationId && a.UserId == senderId, ct);
        if (!isParticipant)
        {
            throw new Exception("Conversation not found or access denied.");
        }

        var message = new Message()
        {
            Id = Ulid.NewUlid().ToString(),
            ConversationId = request.ConversationId,
            SenderId = senderId,
            Text = request.Text,
            Type = MessageType.Text,
            CreatedAt = DateTime.UtcNow,
        };

        await _mongoDbContext.Messages.InsertOneAsync(message, cancellationToken: ct);

        var conversation = await _dbContext.Conversations
            .FindAsync([request.ConversationId], ct);

        if (conversation is not null)
        {
            conversation.LastMessageAt = message.CreatedAt;
            conversation.LastMessagePreview = message.Text?.Length > 50
                ? message.Text[..50] + "..."
                : message.Text;
            conversation.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(ct);
        }

        return new SendMessageResponse(
            message.Id,
            message.ConversationId,
            message.SenderId,
            message.Text,
            message.CreatedAt);

    }
}
