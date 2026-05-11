using MediatR;

namespace Web_messaging_app.Featuers.Contacts.RemoveContact;

public record RemoveContactCommand(Guid ContactUserId) : IRequest;
