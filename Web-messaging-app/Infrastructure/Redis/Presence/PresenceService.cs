using StackExchange.Redis;

namespace Web_messaging_app.Infrastructure.Redis.Presence;

public class PresenceService(IConnectionMultiplexer _connectionMultiplexer) : IPresenceService
{
    private readonly IDatabase redis = _connectionMultiplexer.GetDatabase();
    private static string PresenceKey(Guid userId) => $"Presence:{userId}";


    public async Task SetUserOnlineAsync(Guid userId)
    {
        await redis.StringSetAsync(PresenceKey(userId), "online", TimeSpan.FromSeconds(30));
    }

    public async Task SetUserOfflineAsync(Guid userId)
    {
        await redis.KeyDeleteAsync(PresenceKey(userId));
    }

    public async Task<bool> IsUserOnlineAsync(Guid userId)
    {
        return await redis.KeyExistsAsync(PresenceKey(userId));
    }
    public async Task<List<Guid>> GetOnlineUsersAsync(List<Guid> userIds)
    {
        var batch = redis.CreateBatch();

        var tasks = userIds.Select(id =>
            new { UserId = id, Task = batch.KeyExistsAsync(PresenceKey(id)) }
        ).ToList();

        batch.Execute();
        await Task.WhenAll(tasks.Select(t => t.Task));

        return tasks
            .Where(t => t.Task.Result)
            .Select(t => t.UserId)
            .ToList();
    }


}
