using FluentValidation;

namespace PayrollManagementSystem.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenCommandValidator()
        {
            RuleFor(p => p.AccessToken)
                .NotEmpty().WithMessage("Access Token không được để trống.");

            RuleFor(p => p.RefreshToken)
                .NotEmpty().WithMessage("Refresh Token không được để trống.");
        }
    }
}