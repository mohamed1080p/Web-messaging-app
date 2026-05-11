using MediatR;

namespace Web_messaging_app.Featuers.Contacts.AddContact;

public record AddContactCommand(string ContactUsername, string? Title) : IRequest<AddContactResponse>;