using FluentValidation;

namespace Web_messaging_app.Featuers.Messaging.SendMessage;

public class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
{
    public SendMessageCommandValidator()
    {
        RuleFor(x => x.ConversationId)
            .NotEmpty().WithMessage("ConversationId is required.");

        RuleFor(x => x.Text)
            .NotEmpty().WithMessage("Message text is required.")
            .MaximumLength(2000).WithMessage("Message cannot exceed 2000 characters.")
            .When(x => x.Text is not null);
    }
}
