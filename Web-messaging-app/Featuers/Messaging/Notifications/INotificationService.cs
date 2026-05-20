namespace Web_messaging_app.Featuers.Messaging.Notifications;

public interface INotificationService
{
    Task SendMessageToConversationAsync(
        Guid conversationId,
        List<Guid> participantIds,
        MessageNotification notification,
        CancellationToken ct);
}

public record MessageNotification(
    string MessageId,
    Guid ConversationId,
    Guid SenderId,
    string? Text,
    DateTime CreatedAt
);

public record ConversationUpdatedNotification(
    Guid ConversationId,
    string? LastMessagePreview,
    DateTime? LastMessageAt
);
