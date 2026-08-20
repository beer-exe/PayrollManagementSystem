using FluentValidation;

namespace PayrollManagementSystem.Application.Features.Kpi.Commands.ApprovePhieuKpi
{
    public class ApprovePhieuKpiCommandValidator : AbstractValidator<ApprovePhieuKpiCommand>
    {
        public ApprovePhieuKpiCommandValidator()
        {
            RuleFor(x => x.IdPhieuKpi)
                .NotEmpty().WithMessage("ID phiếu KPI không được để trống.");

            RuleFor(x => x.TaiKhoanIdQuanLy)
                .NotEmpty().WithMessage("ID Quản lý không được để trống.");
        }
    }
}

