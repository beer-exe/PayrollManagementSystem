using FluentValidation;

namespace PayrollManagementSystem.Application.Features.Departments.Commands.ChangePosition
{
    public class ChangePositionCommandValidator : AbstractValidator<ChangePositionCommand>
    {
        public ChangePositionCommandValidator()
        {
            RuleFor(x => x.SoQuyetDinh).NotEmpty().WithMessage("Số quyết định không được để trống.");
            RuleFor(x => x.Cccd).NotEmpty().WithMessage("Mã nhân viên (CCCD) không được để trống.");
            RuleFor(x => x.IdChucVuMoi).NotEmpty().WithMessage("Vui lòng chọn chức vụ mới.");
            RuleFor(x => x.IdBacLuongMoi).NotEmpty().WithMessage("Vui lòng chọn bậc lương áp dụng.");
            RuleFor(x => x.NgayHieuLuc).NotEmpty().WithMessage("Ngày hiệu lực không được để trống.");
        }
    }
}
