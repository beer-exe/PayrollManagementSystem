using FluentValidation;

namespace PayrollManagementSystem.Application.Features.Kpi.Commands.AssignPhieuKpi
{
    public class AssignPhieuKpiCommandValidator : AbstractValidator<AssignPhieuKpiCommand>
    {
        public AssignPhieuKpiCommandValidator()
        {
            RuleFor(x => x.IdPhieuKpi)
                .NotEmpty().WithMessage("ID phiếu KPI không được để trống.");

            RuleFor(x => x.TaiKhoanIdQuanLy)
                .NotEmpty().WithMessage("ID Quản lý không được để trống.");

            RuleFor(x => x.ChiTietKpis)
                .NotEmpty().WithMessage("Danh sách mục tiêu không được để trống.");

            RuleFor(x => x.ChiTietKpis)
                .Must(kpis => kpis != null && kpis.Sum(k => k.TrongSo) == 100)
                .WithMessage("Tổng trọng số của tất cả các mục tiêu phải bằng 100%.");

            RuleForEach(x => x.ChiTietKpis).ChildRules(kpi =>
            {
                kpi.RuleFor(x => x.MucTieu)
                    .NotEmpty().WithMessage("Mục tiêu không được để trống.");
                    
                kpi.RuleFor(x => x.DonViTinh)
                    .NotEmpty().WithMessage("Đơn vị tính không được để trống.");
                    
                kpi.RuleFor(x => x.TrongSo)
                    .GreaterThan(0).WithMessage("Trọng số phải lớn hơn 0.");
                    
                kpi.RuleFor(x => x.ChiTieu)
                    .GreaterThanOrEqualTo(0).WithMessage("Chỉ tiêu không được âm.");
            });
        }
    }
}

