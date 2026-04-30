using AdminPanelBack.DTO;
using FluentValidation;

namespace AdminPanelBack.Validators;

public class UsersMessageDtoValidator : AbstractValidator<UsersMessageDto>
{
    public UsersMessageDtoValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("UserId must be a positive number.");

        RuleFor(x => x.Comment)
            .NotEmpty()
            .MaximumLength(4000);

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Invalid feedback status value.");
    }
}
