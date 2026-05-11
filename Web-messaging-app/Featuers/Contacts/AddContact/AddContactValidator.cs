using FluentValidation;

namespace Web_messaging_app.Featuers.Contacts.AddContact;
public class AddContactValidator:AbstractValidator<AddContactCommand>
{
    public AddContactValidator()
    {
        RuleFor(a => a.ContactUsername)
            .NotEmpty().WithMessage("Username is required.")
            .MaximumLength(50).WithMessage("Username cannot exceed 50 characters.");
    }
}
