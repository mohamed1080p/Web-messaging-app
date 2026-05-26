namespace Web_messaging_app.Infrastructure.Redis.TypingIndicator;

public interface ITypingService
{
    Task StartTypingAsync(Guid userId, Guid conversationId);
    Task StopTypingAsync(Guid userId, Guid conversationId);
    Task<List<Guid>> GetTypingUsersAsync(Guid conversationId);
}
