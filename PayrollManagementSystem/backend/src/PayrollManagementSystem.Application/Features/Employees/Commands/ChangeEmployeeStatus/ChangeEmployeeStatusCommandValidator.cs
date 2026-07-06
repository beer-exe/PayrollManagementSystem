using FluentValidation;

namespace PayrollManagementSystem.Application.Features.Employees.Commands.ChangeEmployeeStatus
{
    public class ChangeEmployeeStatusCommandValidator : AbstractValidator<ChangeEmployeeStatusCommand>
    {
        public ChangeEmployeeStatusCommandValidator()
        {
            RuleFor(x => x.Cccd)
                .NotEmpty().WithMessage("CCCD nhân viên không được để trống.");

            RuleFor(x => x.TrangThaiMoi)
                .IsInEnum().WithMessage("Trạng thái nhân viên không hợp lệ.");

            RuleFor(x => x.LyDo)
                .NotEmpty().WithMessage("Vui lòng cung cấp lý do thay đổi trạng thái.")
                .MaximumLength(255).WithMessage("Lý do không được vượt quá 255 ký tự.");

            RuleFor(x => x.NguoiThayDoi)
                .NotEmpty().WithMessage("Người thực hiện thay đổi không được để trống.");
        }
    }
}