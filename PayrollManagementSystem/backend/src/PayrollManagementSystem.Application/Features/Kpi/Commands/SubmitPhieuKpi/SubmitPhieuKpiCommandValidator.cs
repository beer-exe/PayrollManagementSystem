using PayrollManagementSystem.Application.Features.Kpi.DTOs;
using FluentValidation;

namespace PayrollManagementSystem.Application.Features.Kpi.Commands.SubmitPhieuKpi
{
    public class SubmitPhieuKpiCommandValidator : AbstractValidator<SubmitPhieuKpiCommand>
    {
        public SubmitPhieuKpiCommandValidator()
        {
            RuleFor(x => x.IdPhieuKpi)
                .NotEmpty().WithMessage("ID phiếu KPI không được để trống.");
        }
    }
}

