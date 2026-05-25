using FluentValidation;

namespace PayrollManagementSystem.Application.Features.Users.Commands.CreateUser
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(p => p.TenTaiKhoan).NotEmpty().WithMessage("Tên tài khoản không được để trống.");
            RuleFor(p => p.MatKhau).NotEmpty().MinimumLength(6).WithMessage("Mật khẩu phải có ít nhất 6 ký tự.");
            RuleFor(p => p.IdVaiTro).NotEmpty().WithMessage("Vai trò không được để trống.");
            RuleFor(p => p.Cccd).NotEmpty().WithMessage("CCCD nhân viên không được để trống.");
        }
    }
}
