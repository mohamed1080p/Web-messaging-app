using StackExchange.Redis;
using System.Text.Json;

namespace Web_messaging_app.Infrastructure.Redis.MessageCache;

public class MessageCacheService(IConnectionMultiplexer _connectionMultiplexer) : IMessageCacheService
{
    private readonly IDatabase redis = _connectionMultiplexer.GetDatabase();
    private static string MessagesKey(Guid conversationId) => $"messages:{conversationId}";
    private const int MaxCachedMessages = 50;
    private static readonly TimeSpan CacheExpiry = TimeSpan.FromHours(24);

    public async Task AddMessageAsync(Guid conversationId, CachedMessage message)
    {
        var key = MessagesKey(conversationId);
        var value = JsonSerializer.Serialize(message);
        var score = new DateTimeOffset(message.CreatedAt).ToUnixTimeMilliseconds();

        await redis.SortedSetAddAsync(key, value, score);
        await redis.SortedSetRemoveRangeByRankAsync(key, 0, -(MaxCachedMessages + 1));
        await redis.KeyExpireAsync(key, CacheExpiry);
    }
    public async Task<List<CachedMessage>> GetRecentMessagesAsync(Guid conversationId)
    {
        var key = MessagesKey(conversationId);
        var exists = await redis.KeyExistsAsync(key);
        if (!exists) return [];

        var values = await redis.SortedSetRangeByRankAsync(
            key,
            start: -MaxCachedMessages,
            stop: -1,
            order: Order.Ascending);

        return values
            .Select(v => JsonSerializer.Deserialize<CachedMessage>(v.ToString())!)
            .ToList();
    }

    public async Task RemoveMessageAsync(Guid conversationId, string messageId)
    {
        var key = MessagesKey(conversationId);
        var messages = await redis.SortedSetRangeByRankAsync(key);

        foreach (var item in messages)
        {
            var message = JsonSerializer.Deserialize<CachedMessage>(item.ToString());
            if (message?.MessageId == messageId)
            {
                await redis.SortedSetRemoveAsync(key, item);
                break;
            }
        }
    }

    public async Task InvalidateAsync(Guid conversationId)
    {
        await redis.KeyDeleteAsync(MessagesKey(conversationId));
    }
}
