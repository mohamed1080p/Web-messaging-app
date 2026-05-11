using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Web_messaging_app.Domain.Models.Contacts;
using Web_messaging_app.Infrastructure.Persistence.PostgreSql;

namespace Web_messaging_app.Featuers.Contacts.BlockContact;

public class BlockUserCommandHandler(AppDbContext _dbContext, IHttpContextAccessor _httpContextAccessor) : IRequestHandler<BlockUserCommand>
{
    public async Task Handle(BlockUserCommand command, CancellationToken ct)
    {
        var blockerId = Guid.Parse(
            _httpContextAccessor.HttpContext!.User
                .FindFirstValue(ClaimTypes.NameIdentifier)!);

        if (blockerId == command.UserId)
            throw new Exception("You cannot block yourself.");

        var alreadyBlocked = await _dbContext.UserBlocks
            .AnyAsync(b =>
                b.BlockerUserId == blockerId &&
                b.BlockedUserId == command.UserId, ct);

        if (alreadyBlocked)
            return;

        var block = new UserBlock
        {
            Id = Guid.NewGuid(),
            BlockerUserId = blockerId,
            BlockedUserId = command.UserId,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.UserBlocks.Add(block);
        await _dbContext.SaveChangesAsync(ct);
    }
}
