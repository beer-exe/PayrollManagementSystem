using PayrollManagementSystem.Application.Features.Kpi.DTOs;
using FluentValidation;

namespace PayrollManagementSystem.Application.Features.Kpi.Commands.CreateKyKpi
{
    public class CreateKyKpiCommandValidator : AbstractValidator<CreateKyKpiCommand>
    {
        public CreateKyKpiCommandValidator()
        {
            RuleFor(x => x.TenKyKpi)
                .NotEmpty().WithMessage("Tên kỳ KPI không được để trống.")
                .MaximumLength(100).WithMessage("Tên kỳ KPI không được vượt quá 100 ký tự.");

            RuleFor(x => x.Thang)
                .InclusiveBetween(1, 12).WithMessage("Tháng phải từ 1 đến 12.");

            RuleFor(x => x.Nam)
                .GreaterThan(2000).WithMessage("Năm không hợp lệ.");
        }
    }
}

