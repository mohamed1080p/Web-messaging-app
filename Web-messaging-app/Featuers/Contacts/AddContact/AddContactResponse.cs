namespace Web_messaging_app.Featuers.Contacts.AddContact;

public record AddContactResponse(Guid ContactId, string Username, string DisplayName, string? AvatarUrl);
