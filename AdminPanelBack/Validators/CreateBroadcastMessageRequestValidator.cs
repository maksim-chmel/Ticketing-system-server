using AdminPanelBack.DTO;
using FluentValidation;

namespace AdminPanelBack.Validators;

public class CreateBroadcastMessageRequestValidator : AbstractValidator<CreateBroadcastMessageRequest>
{
    public CreateBroadcastMessageRequestValidator()
    {
        RuleFor(x => x.Message)
            .NotEmpty()
            .MaximumLength(4000);
    }
}
