namespace Web_messaging_app.Infrastructure.Redis.Presence;

public interface IPresenceService
{
    Task SetUserOnlineAsync(Guid userId);
    Task SetUserOfflineAsync(Guid userId);
    Task<bool> IsUserOnlineAsync(Guid userId);
    Task<List<Guid>> GetOnlineUsersAsync(List<Guid> userIds);
}
