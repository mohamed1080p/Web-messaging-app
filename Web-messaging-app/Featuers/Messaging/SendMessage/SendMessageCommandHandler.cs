using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Web_messaging_app.Domain.Models.Messages;
using Web_messaging_app.Featuers.Messaging.Notifications;
using Web_messaging_app.Infrastructure.Persistence.MongoDb;
using Web_messaging_app.Infrastructure.Persistence.PostgreSql;

namespace Web_messaging_app.Featuers.Messaging.SendMessage;

public class SendMessageCommandHandler
    (AppDbContext _dbContext,
    MongoDbContext _mongoDbContext,
    IHttpContextAccessor _httpContextAccessor,
    INotificationService _notificationService) : IRequestHandler<SendMessageCommand, SendMessageResponse>
{
    public async Task<SendMessageResponse> Handle(SendMessageCommand command, CancellationToken ct)
    {
        var currentUserId = Guid.Parse(
            _httpContextAccessor.HttpContext!.User
                .FindFirstValue(ClaimTypes.NameIdentifier)!);

        var participantIds = await _dbContext.conversationParticipants
            .Where(cp => cp.ConversationId == command.ConversationId)
            .Select(cp => cp.UserId)
            .ToListAsync(ct);

        if (!participantIds.Contains(currentUserId))
            throw new Exception("Conversation not found or access denied.");

        // create and save message to MongoDB
        var message = new Message
        {
            Id = Ulid.NewUlid().ToString(),
            ConversationId = command.ConversationId,
            SenderId = currentUserId,
            Type = MessageType.Text,
            Text = command.Text,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow
        };

        await _mongoDbContext.Messages.InsertOneAsync(message, cancellationToken: ct);

        // update conversation in PostgreSQL
        var conversation = await _dbContext.Conversations
            .FindAsync([command.ConversationId], ct);

        if (conversation is not null)
        {
            conversation.LastMessageAt = message.CreatedAt;
            conversation.LastMessagePreview = message.Text?.Length > 50
                ? message.Text[..50] + "..."
                : message.Text;
            conversation.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(ct);
        }

        // broadcast to all participants
        await _notificationService.SendMessageToConversationAsync(
            command.ConversationId,
            participantIds,
            new MessageNotification(
                message.Id,
                message.ConversationId,
                message.SenderId,
                message.Text,
                message.CreatedAt),
            ct);

        return new SendMessageResponse(
            message.Id,
            message.ConversationId,
            message.SenderId,
            message.Text,
            message.CreatedAt
        );
    }
}
