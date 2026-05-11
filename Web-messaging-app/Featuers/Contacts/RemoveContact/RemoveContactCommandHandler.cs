using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Web_messaging_app.Infrastructure.Persistence.PostgreSql;

namespace Web_messaging_app.Featuers.Contacts.RemoveContact;

public class RemoveContactCommandHandler(AppDbContext _dbContext, IHttpContextAccessor _httpContextAccessor) : IRequestHandler<RemoveContactCommand>
{
    public async Task Handle(RemoveContactCommand request, CancellationToken ct)
    {
        var ownerId = Guid.Parse(
            _httpContextAccessor.HttpContext!.User
                .FindFirstValue(ClaimTypes.NameIdentifier)!);

        var contact = await _dbContext.Contacts
            .SingleOrDefaultAsync(c => c.OwnerId == ownerId && c.ContactUserId == request.ContactUserId, ct);

        if (contact is null)
            return;

        _dbContext.Contacts.Remove(contact);
        await _dbContext.SaveChangesAsync(ct);
    }
}
