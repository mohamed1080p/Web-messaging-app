using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Web_messaging_app.Infrastructure.Persistence.PostgreSql;

namespace Web_messaging_app.Featuers.Messaging.Hubs;

[Authorize]
public class ChatHub(AppDbContext _dbContext) : Hub
{

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User!.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // Each user joins their own personal group on connect
        // This is used to receive notifications regardless of which screen they are on
        await Groups.AddToGroupAsync(Context.ConnectionId, $"user:{userId}");

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User!.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // Update LastSeenAt when user disconnects
        var user = await _dbContext.Users.FindAsync(Guid.Parse(userId));
        if (user is not null)
        {
            user.LastSeenAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
        }

        await base.OnDisconnectedAsync(exception);
    }

    // Called when the user opens a conversation
    public async Task JoinConversation(string conversationId)
    {
        var userId = Guid.Parse(
            Context.User!.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // Verify the user is a participant before joining the group
        var isParticipant = await _dbContext.conversationParticipants
            .AnyAsync(cp =>
                cp.ConversationId == Guid.Parse(conversationId) &&
                cp.UserId == userId);

        if (!isParticipant)
            throw new HubException("Access denied.");

        await Groups.AddToGroupAsync(Context.ConnectionId, $"conversation:{conversationId}");
    }

    // Called when the user closes a conversation or navigates away
    public async Task LeaveConversation(string conversationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"conversation:{conversationId}");
    }

    // Called when the user starts typing
    public async Task StartTyping(string conversationId)
    {
        var userId = Context.User!.FindFirstValue(ClaimTypes.NameIdentifier)!;

        await Clients.OthersInGroup($"conversation:{conversationId}")
            .SendAsync("UserTyping", new
            {
                ConversationId = conversationId,
                UserId = userId
            });
    }

    // Called when the user stops typing
    public async Task StopTyping(string conversationId)
    {
        var userId = Context.User!.FindFirstValue(ClaimTypes.NameIdentifier)!;

        await Clients.OthersInGroup($"conversation:{conversationId}")
            .SendAsync("UserStoppedTyping", new
            {
                ConversationId = conversationId,
                UserId = userId
            });
    }
}
