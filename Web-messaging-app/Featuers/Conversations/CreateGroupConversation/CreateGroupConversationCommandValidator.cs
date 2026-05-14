using FluentValidation;
namespace Web_messaging_app.Featuers.Conversations.CreateGroupConversation;

public class CreateGroupConversationCommandValidator
    : AbstractValidator<CreateGroupConversationCommand>
{
    public CreateGroupConversationCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Group title is required.")
            .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(100).WithMessage("Description cannot exceed 500 characters.")
            .When(x => x.Description is not null);

        RuleFor(x => x.ParticipantUserIds)
            .NotEmpty().WithMessage("At least one participant is required.")
            .Must(ids => ids.Count >= 1)
            .WithMessage("A group must have at least one other participant.");
    }
}
