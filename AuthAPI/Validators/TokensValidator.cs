using AuthAPI.Models.Dto;
using FluentValidation;

namespace AuthAPI.Validators;

public class TokensValidator: AbstractValidator<TokensDto>
{
    public TokensValidator()
    {
        CascadeMode = CascadeMode.Stop;

        RuleFor(p => p.Token)
            .NotEmpty().WithMessage("Access token is required.");

        RuleFor(p => p.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required.");
    }
}
