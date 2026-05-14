using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Web_messaging_app.Infrastructure.Persistence.PostgreSql;

namespace Web_messaging_app.Featuers.Conversations.GetConversationDetails;

public class GetConversationDetailsQueryHandler(AppDbContext _dbContext, IHttpContextAccessor _httpContextAccessor) :
    IRequestHandler<GetConversationDetailsQuery, ConversationDetailsDto>
{
    public async Task<ConversationDetailsDto> Handle(GetConversationDetailsQuery request, CancellationToken ct)
    {
        var currentUserId = Guid.Parse(
            _httpContextAccessor.HttpContext!.User
                .FindFirstValue(ClaimTypes.NameIdentifier)!);

        var conversation = await _dbContext.Conversations
            .Where(c => c.Id == request.ConversationId)
            .Where(c => c.Participants.Any(p => p.UserId == currentUserId))
            .Select(c => new ConversationDetailsDto(
                c.Id,
                c.Type,
                c.Title,
                c.Description,
                c.AvatarUrl,
                c.CreatedAt,
                c.Participants.Select(p => new ParticipantDto(
                    p.UserId,
                    p.User.UserName,
                    p.User.AvatarUrl,
                    p.Role,
                    p.JoinedAt)).ToList()))
            .FirstOrDefaultAsync(ct);

        if (conversation is null)
            throw new Exception("Conversation not found.");

        return conversation;
    }
}
