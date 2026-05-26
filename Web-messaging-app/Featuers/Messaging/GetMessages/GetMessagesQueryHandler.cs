using MediatR;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System.Security.Claims;
using Web_messaging_app.Domain.Models.Conversations;
using Web_messaging_app.Infrastructure.Persistence.MongoDb;
using Web_messaging_app.Infrastructure.Persistence.PostgreSql;
using Web_messaging_app.Infrastructure.Redis.MessageCache;

namespace Web_messaging_app.Featuers.Messaging.GetMessages;

public class GetMessagesQueryHandler(
    AppDbContext _dbContext,
    MongoDbContext _mongoDbContext,
    IHttpContextAccessor _httpContextAccessor,
    IMessageCacheService _messageCache,
    ILogger<GetMessagesQueryHandler> _logger)
    : IRequestHandler<GetMessagesQuery, GetMessagesResponse>
{
    public async Task<GetMessagesResponse> Handle(GetMessagesQuery request, CancellationToken ct)
    {
        var currentUserId = Guid.Parse(
            _httpContextAccessor.HttpContext!.User
                .FindFirstValue(ClaimTypes.NameIdentifier)!);

        var participant = await _dbContext.conversationParticipants
            .SingleOrDefaultAsync(cp =>
                cp.ConversationId == request.ConversationId &&
                cp.UserId == currentUserId, ct);

        if (participant is null)
            throw new Exception("Conversation not found or access denied.");

        // Page 1 — try Redis first
        if (request.Page == 1)
        {
            try
            {
                var cached = await _messageCache.GetRecentMessagesAsync(request.ConversationId);

                if (cached.Count > 0)
                {
                    await UpdateLastReadAtAsync(participant, ct);

                    return new GetMessagesResponse(
                        cached.Select(m => new MessageDto(
                            m.MessageId,
                            m.SenderId,
                            m.IsDeleted ? null : m.Text,
                            m.IsDeleted,
                            m.CreatedAt)).ToList(),
                        request.Page,
                        request.PageSize,
                        true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis unavailable, falling back to MongoDB for conversation {ConversationId}", request.ConversationId);
            }
        }

        // MongoDB fallback — used when:
        // 1. Redis is down
        // 2. Cache miss on page 1
        // 3. Page > 1 (older messages)
        var pageSize = Math.Min(request.PageSize, 50);

        var messages = await _mongoDbContext.Messages
            .Find(m => m.ConversationId == request.ConversationId)
            .SortByDescending(m => m.CreatedAt)
            .Skip((request.Page - 1) * pageSize)
            .Limit(pageSize + 1)
            .ToListAsync(ct);

        var hasMore = messages.Count > pageSize;
        if (hasMore)
            messages.RemoveAt(messages.Count - 1);

        // Populate Redis on page 1 cache miss
        if (request.Page == 1 && messages.Count > 0)
        {
            try
            {
                foreach (var msg in messages)
                {
                    await _messageCache.AddMessageAsync(request.ConversationId, new CachedMessage(
                        msg.Id,
                        msg.ConversationId,
                        msg.SenderId,
                        msg.Text,
                        msg.IsDeleted,
                        msg.CreatedAt));
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis unavailable, cache population skipped for conversation {ConversationId}", request.ConversationId);
            }
        }

        await UpdateLastReadAtAsync(participant, ct);

        return new GetMessagesResponse(
            messages.Select(m => new MessageDto(
                m.Id,
                m.SenderId,
                m.IsDeleted ? null : m.Text,
                m.IsDeleted,
                m.CreatedAt)).ToList(),
            request.Page,
            pageSize,
            hasMore);
    }

    private async Task UpdateLastReadAtAsync(ConversationParticipant participant, CancellationToken ct)
    {
        participant.LastReadAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(ct);
    }
}