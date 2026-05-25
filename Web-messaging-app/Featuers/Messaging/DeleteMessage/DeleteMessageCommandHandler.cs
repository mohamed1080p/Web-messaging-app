using MediatR;
using MongoDB.Driver;
using System.Security.Claims;
using Web_messaging_app.Domain.Models.Messages;
using Web_messaging_app.Infrastructure.Persistence.MongoDb;
using Web_messaging_app.Infrastructure.Redis.MessageCache;

namespace Web_messaging_app.Featuers.Messaging.DeleteMessage;

public class DeleteMessageCommandHandler(
    MongoDbContext _mongoDbContext,
    IHttpContextAccessor _httpContextAccessor,
    IMessageCacheService _messageCache) : IRequestHandler<DeleteMessageCommand>
{
    public async Task Handle(DeleteMessageCommand request, CancellationToken ct)
    {
        var currentUserId = Guid.Parse(
            _httpContextAccessor.HttpContext!.User
                .FindFirstValue(ClaimTypes.NameIdentifier)!);

        var message = await _mongoDbContext.Messages
           .Find(m => m.Id == request.MessageId && m.ConversationId == request.ConversationId)
           .FirstOrDefaultAsync(ct);

        if (message is null)
        {
            return;
        }
        if (message.SenderId != currentUserId)
        {
            throw new Exception("You can only delete your own messages.");
        }
        var update = Builders<Message>.Update
            .Set(m => m.IsDeleted, true)
            .Set(m => m.DeletedAt, DateTime.UtcNow)
            .Set(m => m.Text, null);

        await _mongoDbContext.Messages.UpdateOneAsync(
            m => m.Id == request.MessageId,
            update,
            cancellationToken: ct);

        try
        {
            await _messageCache.RemoveMessageAsync(request.ConversationId, request.MessageId);
        }
        catch (Exception ex)
        {

        }
    }
}
