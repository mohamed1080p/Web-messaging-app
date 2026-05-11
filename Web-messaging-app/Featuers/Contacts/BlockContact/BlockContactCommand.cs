using MediatR;

namespace Web_messaging_app.Featuers.Contacts.BlockContact;

public record BlockUserCommand(Guid UserId) : IRequest;

