using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Web_messaging_app.Domain.Models.Conversations;
using Web_messaging_app.Infrastructure.Persistence.PostgreSql;

namespace Web_messaging_app.Featuers.Conversations.GetOrCreateDirectConversation;

public class GetOrCreateDirectConversationCommandHandler(AppDbContext _dbContext, IHttpContextAccessor _httpContextAccessor) :
    IRequestHandler<GetOrCreateDirectConversationCommand
    , GetOrCreateDirectConversationResponse>
{
    public async Task<GetOrCreateDirectConversationResponse> Handle(GetOrCreateDirectConversationCommand request, CancellationToken ct)
    {
        var ownerId = Guid.Parse(_httpContextAccessor.HttpContext!.
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var conversation = await _dbContext.Conversations
            .Where(c => c.Type == ConversationType.Direct)
            .Where(c => c.Participants.Any(p => p.UserId == ownerId))
            .Where(c => c.Participants.Any(p => p.UserId == request.ContactUserId))
            .FirstOrDefaultAsync(ct);

        if (conversation is not null)
        {
            return new GetOrCreateDirectConversationResponse(
                conversation.Id,
                conversation.Type,
                conversation.Title,
                conversation.AvatarUrl,
                conversation.CreatedAt);
        }

        var contactExists = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == request.ContactUserId, ct);

        if (contactExists is null)
            throw new Exception("User not found.");

        var contact = await _dbContext.Contacts.
            FirstOrDefaultAsync(a => a.OwnerId == ownerId && a.ContactUserId == request.ContactUserId, ct);

        var contactTitle = contact?.Name ?? contactExists.DisplayName;
        var utcNow = DateTime.UtcNow;

        var NewConversation = new Conversation()
        {
            Id = Guid.NewGuid(),
            Type = ConversationType.Direct,
            AvatarUrl = contactExists.AvatarUrl,
            CreatedAt = utcNow,
            CreatedByUserId = ownerId,
            Title = contactTitle,
        };

        var participants = new List<ConversationParticipant>
        {
            new()
            {
                Id=Guid.NewGuid(),
                ConversationId=NewConversation.Id,
                UserId=ownerId,
                Role=ParticipantRole.Member,
                JoinedAt=utcNow
            },

            new()
            {
                Id=Guid.NewGuid(),
                ConversationId=NewConversation.Id,
                UserId=request.ContactUserId,
                Role=ParticipantRole.Member,
                JoinedAt=utcNow
            }
        };

        _dbContext.Conversations.Add(NewConversation);
        _dbContext.conversationParticipants.AddRange(participants);
        await _dbContext.SaveChangesAsync(ct);

        return new GetOrCreateDirectConversationResponse(
            NewConversation.Id,
            NewConversation.Type,
            NewConversation.Title,
            NewConversation.AvatarUrl,
            NewConversation.CreatedAt);

    }
}
