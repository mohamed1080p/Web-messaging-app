using MediatR;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System.Security.Claims;
using Web_messaging_app.Infrastructure.Persistence.MongoDb;
using Web_messaging_app.Infrastructure.Persistence.PostgreSql;

namespace Web_messaging_app.Featuers.Messaging.GetMessages;

public class GetMessagesQueryHandler(
    AppDbContext _dbContext,
    MongoDbContext _mongoDbContext,
    IHttpContextAccessor _httpContextAccessor)
    : IRequestHandler<GetMessagesQuery, GetMessagesResponse>
{
    public async Task<GetMessagesResponse> Handle(GetMessagesQuery request, CancellationToken ct)
    {
        var currentUserId = Guid.Parse(
            _httpContextAccessor.HttpContext!.User
                .FindFirstValue(ClaimTypes.NameIdentifier)!);

        var Participant = await _dbContext.conversationParticipants.
            SingleOrDefaultAsync(a => a.ConversationId == request.ConversationId && a.UserId == currentUserId, ct);
        if (Participant is null)
        {
            throw new Exception("Conversation not found or access denied.");
        }

        var pageSize = Math.Min(request.PageSize, 50);

        var messages = await _mongoDbContext.Messages
            .Find(m => m.ConversationId == request.ConversationId)
            .SortByDescending(m => m.CreatedAt)
            .Skip((request.Page - 1) * pageSize)
            .Limit(pageSize + 1) // fetch one extra to check if there are more pages
            .ToListAsync(ct);

        // check if there are more pages
        var hasMore = messages.Count > pageSize;
        if (hasMore)
        {
            messages.RemoveAt(messages.Count - 1);
        }

        Participant.LastReadAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(ct);

        return new GetMessagesResponse(
            messages.Select(m => new MessageDto(
                m.Id,
                m.SenderId,
                m.IsDeleted ? null : m.Text,
                m.IsDeleted,
                m.CreatedAt)).ToList(),
            request.Page,
            pageSize,
            hasMore
        );

    }
}
