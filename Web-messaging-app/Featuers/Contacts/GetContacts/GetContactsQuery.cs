using MediatR;

namespace Web_messaging_app.Featuers.Contacts.GetContacts;

public record GetContactsQuery : IRequest<List<ContactDto>>;

public record ContactDto(
    Guid UserId,
    string Username,
    string DisplayName,
    string? AvatarUrl
);

