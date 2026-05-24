using StackExchange.Redis;

namespace Web_messaging_app.Infrastructure.Redis.TypingIndicator;

public class TypingService(IConnectionMultiplexer _connectionMultiplexer) : ITypingService
{
    private readonly IDatabase redis = _connectionMultiplexer.GetDatabase();
    private static string TypingKey(Guid conversationId, Guid userId) => $"typing:{conversationId}:{userId}";
    private static string TypingSetKey(Guid conversationId) => $"typing:{conversationId}";
    public async Task StartTypingAsync(Guid userId, Guid conversationId)
    {
        await redis.SetAddAsync(TypingSetKey(conversationId), userId.ToString());
        await redis.StringSetAsync(TypingKey(conversationId, userId), "typing", TimeSpan.FromSeconds(5));
    }

    public async Task StopTypingAsync(Guid userId, Guid conversationId)
    {
        await redis.SetRemoveAsync(TypingSetKey(conversationId), userId.ToString());
        await redis.KeyDeleteAsync(TypingKey(conversationId, userId));
    }
    public async Task<List<Guid>> GetTypingUsersAsync(Guid conversationId)
    {
        var members = await redis.SetMembersAsync(TypingSetKey(conversationId));
        var users = new List<Guid>();
        foreach (var item in members)
        {
            var userId = Guid.Parse(item.ToString());
            var stillTyping = await redis.KeyExistsAsync(TypingKey(conversationId, userId));
            if (stillTyping)
            {
                users.Add(userId);
            }
            else
            {
                await redis.SetRemoveAsync(TypingSetKey(conversationId), item);
            }
        }
        return users;
    }

}
