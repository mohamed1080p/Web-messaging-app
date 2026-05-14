using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Web_messaging_app.Domain.Models.Conversations;
using Web_messaging_app.Infrastructure.Persistence.PostgreSql;

namespace Web_messaging_app.Featuers.Conversations.GetConversations;

public class GetConversationsQueryHandler(AppDbContext _dbContext, IHttpContextAccessor _httpContextAccessor) : IRequestHandler<GetConversationsQuery, List<ConversationListDto>>
{
    public async Task<List<ConversationListDto>> Handle(GetConversationsQuery request, CancellationToken ct)
    {
        var currentUserId = Guid.Parse(
            _httpContextAccessor.HttpContext!.User
                .FindFirstValue(ClaimTypes.NameIdentifier)!);


        return await _dbContext.Conversations
    .Where(c => c.Participants.Any(p => p.UserId == currentUserId))
    .OrderByDescending(c => c.LastMessageAt)
    .Select(c => new ConversationListDto(
        c.Id,
        c.Type,
        c.Type == ConversationType.Direct
            ? c.Participants
                .Where(p => p.UserId != currentUserId)
                .Select(p =>
                    // Check if current user has given this contact a nickname
                    _dbContext.Contacts
                        .Where(contact =>
                            contact.OwnerId == currentUserId &&
                            contact.ContactUserId == p.UserId)
                        .Select(contact => contact.Name)
                        .FirstOrDefault()
                    ?? p.User.DisplayName) // fallback to display name if no nickname
                .FirstOrDefault()
            : c.Title,
        c.Type == ConversationType.Direct
            ? c.Participants
                .Where(p => p.UserId != currentUserId)
                .Select(p => p.User.AvatarUrl)
                .FirstOrDefault()
            : c.AvatarUrl,
        c.LastMessagePreview,
        c.LastMessageAt))
    .ToListAsync(ct);
    }
}
