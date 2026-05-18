using FluentValidation;

namespace PayrollManagementSystem.Application.Features.Auth.Commands.Login
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(p => p.TenTaiKhoan)
                .NotEmpty().WithMessage("Tên tài khoản không được để trống.");

            RuleFor(p => p.MatKhau)
                .NotEmpty().WithMessage("Mật khẩu không được để trống.");
        }
    }
}
