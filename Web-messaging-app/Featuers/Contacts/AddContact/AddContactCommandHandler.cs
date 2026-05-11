using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Web_messaging_app.Domain.Models.Contacts;
using Web_messaging_app.Infrastructure.Persistence.PostgreSql;

namespace Web_messaging_app.Featuers.Contacts.AddContact;

public class AddContactCommandHandler(AppDbContext _dbContext, IHttpContextAccessor _httpContextAccessor) : IRequestHandler<AddContactCommand, AddContactResponse>
{
    public async Task<AddContactResponse> Handle(AddContactCommand request, CancellationToken ct)
    {
        var ownerId = Guid.Parse(_httpContextAccessor.HttpContext!.
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var contactUser = await _dbContext.Users.
            SingleOrDefaultAsync(u => u.UserName == request.ContactUsername, ct);

        if (contactUser is null)
            throw new Exception("User not found.");

        if (contactUser.Id == ownerId)
            throw new Exception("You cannot add yourself as a contact.");

        var alreadyExists = await _dbContext.Contacts
            .AnyAsync(c => c.OwnerId == ownerId && c.ContactUserId == contactUser.Id, ct);

        if (alreadyExists)
            throw new Exception("User is already in your contacts.");

        var title = request.Title ?? contactUser.DisplayName;

        var contact = new Contact
        {
            Id = Guid.NewGuid(),
            OwnerId = ownerId,
            ContactUserId = contactUser.Id,
            CreatedAt = DateTime.UtcNow,
            Name=title
        };

        _dbContext.Contacts.Add(contact);
        await _dbContext.SaveChangesAsync(ct);

        return new AddContactResponse(
            contactUser.Id,
            contactUser.UserName,
            title,
            contactUser.AvatarUrl
        );
    }
}
