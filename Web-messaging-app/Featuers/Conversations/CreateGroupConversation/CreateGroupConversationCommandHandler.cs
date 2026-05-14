using MediatR;
using System.Security.Claims;
using Web_messaging_app.Domain.Models.Conversations;
using Web_messaging_app.Featuers.Conversations.GetOrCreateDirectConversation;
using Web_messaging_app.Infrastructure.Persistence.PostgreSql;

namespace Web_messaging_app.Featuers.Conversations.CreateGroupConversation;

public class CreateGroupConversationCommandHandler(AppDbContext _dbContext, IHttpContextAccessor _httpContextAccessor) :
    IRequestHandler<CreateGroupConversationCommand, ConversationResponse>
{
    public async Task<ConversationResponse> Handle(CreateGroupConversationCommand request, CancellationToken ct)
    {
        var currentUserId = Guid.Parse(
            _httpContextAccessor.HttpContext!.User
                .FindFirstValue(ClaimTypes.NameIdentifier)!);

        var conversation = new Conversation
        {
            Id = Guid.NewGuid(),
            Type = ConversationType.Group,
            Title = request.Title,
            Description = request.Description,
            CreatedByUserId = currentUserId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            AvatarUrl = request.AvatarUrl
        };

        var participants = new List<ConversationParticipant>
        {
            new()
            {
                Id = Guid.NewGuid(),
                ConversationId = conversation.Id,
                UserId = currentUserId,
                Role = ParticipantRole.Owner,
                JoinedAt = DateTime.UtcNow
            }
        };


        var otherParticipants = request.ParticipantUserIds
            .Distinct()
            .Where(id => id != currentUserId)
            .Select(userId => new ConversationParticipant
            {
                Id = Guid.NewGuid(),
                ConversationId = conversation.Id,
                UserId = userId,
                Role = ParticipantRole.Member,
                JoinedAt = DateTime.UtcNow
            });

        participants.AddRange(otherParticipants);

        _dbContext.Conversations.Add(conversation);
        _dbContext.conversationParticipants.AddRange(participants);
        await _dbContext.SaveChangesAsync(ct);

        return new ConversationResponse(
            conversation.Id,
            conversation.Type,
            conversation.Title,
            conversation.AvatarUrl,
            conversation.CreatedAt);
    }
}
