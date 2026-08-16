using PayrollManagementSystem.Application.Features.Kpi.DTOs;
using FluentValidation;

namespace PayrollManagementSystem.Application.Features.Kpi.Commands.SaveChiTietKpi
{
    public class SaveChiTietKpiCommandValidator : AbstractValidator<SaveChiTietKpiCommand>
    {
        public SaveChiTietKpiCommandValidator()
        {
            RuleFor(x => x.IdPhieuKpi)
                .NotEmpty().WithMessage("ID phiếu KPI không được để trống.");

            RuleFor(x => x.ChiTietKpis)
                .NotEmpty().WithMessage("Danh sách tiến độ không được để trống.");

            RuleForEach(x => x.ChiTietKpis).ChildRules(kpi =>
            {
                kpi.RuleFor(x => x.IdChiTietKpi)
                    .NotEmpty().WithMessage("ID chi tiết KPI không được để trống.");
                    
                kpi.RuleFor(x => x.ThucTe)
                    .GreaterThanOrEqualTo(0).WithMessage("Kết quả thực tế không được âm.");
            });
        }
    }
}

