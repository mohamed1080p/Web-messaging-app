using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Web_messaging_app.Infrastructure.Persistence.PostgreSql;
using Web_messaging_app.Infrastructure.Redis.Presence;
using Web_messaging_app.Infrastructure.Redis.TypingIndicator;

namespace Web_messaging_app.Featuers.Messaging.Hubs;

[Authorize]
public class ChatHub(AppDbContext _dbContext,
    IPresenceService _presenceService,
    ITypingService _typingService) : Hub
{

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User!.FindFirstValue(ClaimTypes.NameIdentifier)!;

        await _presenceService.SetUserOnlineAsync(Guid.Parse(userId));

        await Groups.AddToGroupAsync(Context.ConnectionId, $"user:{userId}");

        await Clients.Others.SendAsync("UserOnline", new { UserId = userId });

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User!.FindFirstValue(ClaimTypes.NameIdentifier)!;

        await _presenceService.SetUserOfflineAsync(Guid.Parse(userId));

        var user = await _dbContext.Users.FindAsync(Guid.Parse(userId));
        if (user is not null)
        {
            user.LastSeenAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
        }

        await Clients.Others.SendAsync("UserOffline", new { UserId = userId });

        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinConversation(string conversationId)
    {
        var userId = Guid.Parse(
            Context.User!.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var isParticipant = await _dbContext.conversationParticipants
            .AnyAsync(cp =>
                cp.ConversationId == Guid.Parse(conversationId) &&
                cp.UserId == userId);

        if (!isParticipant)
            throw new HubException("Access denied.");

        await Groups.AddToGroupAsync(Context.ConnectionId, $"conversation:{conversationId}");
    }

    public async Task LeaveConversation(string conversationId)
    {
        var userId = Guid.Parse(
           Context.User!.FindFirstValue(ClaimTypes.NameIdentifier)!);

        await _typingService.StopTypingAsync(userId, Guid.Parse(conversationId));
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"conversation:{conversationId}");
    }

    public async Task StartTyping(string conversationId)
    {
        var userId = Guid.Parse(
            Context.User!.FindFirstValue(ClaimTypes.NameIdentifier)!);

        await _typingService.StartTypingAsync(userId, Guid.Parse(conversationId));

        await Clients.OthersInGroup($"conversation:{conversationId}")
            .SendAsync("UserTyping", new
            {
                ConversationId = conversationId,
                UserId = userId
            });
    }

    public async Task StopTyping(string conversationId)
    {
        var userId = Guid.Parse(
            Context.User!.FindFirstValue(ClaimTypes.NameIdentifier)!);

        await _typingService.StopTypingAsync(userId, Guid.Parse(conversationId));

        await Clients.OthersInGroup($"conversation:{conversationId}")
            .SendAsync("UserStoppedTyping", new
            {
                ConversationId = conversationId,
                UserId = userId
            });
    }
    // Client sends heartbeat every 30 seconds to keep presence alive
    public async Task Heartbeat()
    {
        var userId = Guid.Parse(
            Context.User!.FindFirstValue(ClaimTypes.NameIdentifier)!);

        await _presenceService.SetUserOnlineAsync(userId);
    }
}
