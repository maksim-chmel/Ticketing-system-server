using AdminPanelBack.DTO;
using FluentValidation;

namespace AdminPanelBack.Validators;

public class UpdateUserCommentRequestValidator : AbstractValidator<UpdateUserCommentRequest>
{
    public UpdateUserCommentRequestValidator()
    {
        RuleFor(x => x.Comment)
            .NotEmpty()
            .MaximumLength(2000);
    }
}
