namespace Web_messaging_app.Infrastructure.Redis.MessageCache;

public interface IMessageCacheService
{
    Task AddMessageAsync(Guid conversationId, CachedMessage message);
    Task RemoveMessageAsync(Guid conversationId, string messageId);
    Task<List<CachedMessage>> GetRecentMessagesAsync(Guid conversationId);
    Task InvalidateAsync(Guid conversationId);
}

public record CachedMessage(
    string MessageId,
    Guid ConversationId,
    Guid SenderId,
    string? Text,
    bool IsDeleted,
    DateTime CreatedAt
);