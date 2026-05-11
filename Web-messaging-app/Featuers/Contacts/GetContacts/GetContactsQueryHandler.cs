using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Web_messaging_app.Infrastructure.Persistence.PostgreSql;

namespace Web_messaging_app.Featuers.Contacts.GetContacts;

public class GetContactsQueryHandler(AppDbContext _dbContext, IHttpContextAccessor _httpContextAccessor) : IRequestHandler<GetContactsQuery, List<ContactDto>>
{
    public async Task<List<ContactDto>> Handle(GetContactsQuery query, CancellationToken ct)
    {
        var ownerId = Guid.Parse(
            _httpContextAccessor.HttpContext!.User
                .FindFirstValue(ClaimTypes.NameIdentifier)!);

        return await _dbContext.Contacts
            .Where(c => c.OwnerId == ownerId)
            .Select(c => new ContactDto(
                c.ContactUser.Id,
                c.ContactUser.UserName,
                c.ContactUser.DisplayName,
                c.ContactUser.AvatarUrl))
            .ToListAsync(ct);
    }
}
