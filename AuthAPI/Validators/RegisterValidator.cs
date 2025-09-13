using AuthAPI.Models.Dto;
using FluentValidation;

namespace AuthAPI.Validators;

public class RegisterValidator: AbstractValidator<RegisterDto>
{
    public RegisterValidator()
    {
        CascadeMode = CascadeMode.Stop;

        RuleFor(p => p.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters long.")
            .MaximumLength(20).WithMessage("Username must not exceed 20 characters.");

        RuleFor(p => p.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");

        RuleFor(p => p.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Please enter a valid email address.");
    }
}
