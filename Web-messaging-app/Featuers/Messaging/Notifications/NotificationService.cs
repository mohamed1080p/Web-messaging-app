using Microsoft.AspNetCore.SignalR;
using Web_messaging_app.Featuers.Messaging.Hubs;

namespace Web_messaging_app.Featuers.Messaging.Notifications;

public class NotificationService(IHubContext<ChatHub> _hubContext) : INotificationService
{
    public async Task SendMessageToConversationAsync(Guid conversationId,
        List<Guid> participantIds, MessageNotification notification, CancellationToken ct)
    {
        // Event 1 — send full message to users who have this conversation open
        await _hubContext.Clients
            .Group($"conversation:{conversationId}")
            .SendAsync("ReceiveMessage", notification, ct);

        // Event 2 — notify ALL participants via their personal group
        // so their conversation list updates regardless of which screen they are on
        var conversationUpdate = new ConversationUpdatedNotification(
            notification.ConversationId,
            notification.Text?.Length > 50
                ? notification.Text[..50] + "..."
                : notification.Text,
            notification.CreatedAt);

        foreach (var participantId in participantIds)
        {
            await _hubContext.Clients
                .Group($"user:{participantId}")
                .SendAsync("ConversationUpdated", conversationUpdate, ct);
        }
    }
}
