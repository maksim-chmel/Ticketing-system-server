using AdminPanelBack.DTO;
using FluentValidation;

namespace AdminPanelBack.Validators;

public class UpdateFeedbackStatusRequestValidator : AbstractValidator<UpdateFeedbackStatusRequest>
{
    public UpdateFeedbackStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Invalid feedback status value.");
    }
}
