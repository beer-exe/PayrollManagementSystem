using FluentValidation;

namespace PayrollManagementSystem.Application.Features.Users.Commands.UpdateUserRole
{
    public class UpdateUserRoleCommandValidator : AbstractValidator<UpdateUserRoleCommand>
    {
        public UpdateUserRoleCommandValidator()
        {
            RuleFor(x => x.IdTaiKhoan)
                .NotEmpty().WithMessage("ID tài khoản không được để trống.");

            RuleFor(x => x.IdVaiTroMoi)
                .NotEmpty().WithMessage("Vui lòng chọn vai trò mới.");
        }
    }
}
