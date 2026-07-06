using FluentValidation;

namespace PayrollManagementSystem.Application.Features.Departments.Commands.AdjustSalary
{
    public class AdjustSalaryCommandValidator : AbstractValidator<AdjustSalaryCommand>
    {
        public AdjustSalaryCommandValidator()
        {
            RuleFor(x => x.SoQuyetDinh).NotEmpty().WithMessage("Số quyết định không được để trống.");
            RuleFor(x => x.Cccd).NotEmpty().WithMessage("Mã nhân viên (CCCD) không được để trống.");
            RuleFor(x => x.IdBacLuongMoi).NotEmpty().WithMessage("Vui lòng chọn bậc lương mới.");
            RuleFor(x => x.NgayHieuLuc).NotEmpty().WithMessage("Ngày hiệu lực không được để trống.");
        }
    }
}
