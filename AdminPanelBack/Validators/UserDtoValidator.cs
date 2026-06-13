using AdminPanelBack.DTO;
using FluentValidation;

namespace AdminPanelBack.Validators;

public class UserDtoValidator : AbstractValidator<UserDto>
{
    public UserDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Phone)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.LastName)
            .MaximumLength(100)
            .When(x => x.LastName != null);

        RuleFor(x => x.Username)
            .MaximumLength(100)
            .When(x => x.Username != null);

        RuleFor(x => x.Comments)
            .MaximumLength(2000)
            .When(x => x.Comments != null);
    }
}
