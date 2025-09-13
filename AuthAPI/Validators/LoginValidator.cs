using AuthAPI.Models.Dto;
using FluentValidation;

namespace AuthAPI.Validators;

public class LoginValidator: AbstractValidator<LoginDto>
{
    public LoginValidator()
    {
        CascadeMode = CascadeMode.Stop;

        RuleFor(p => p.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");

        RuleFor(p => p.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Please enter a valid email address.");
    }
}
