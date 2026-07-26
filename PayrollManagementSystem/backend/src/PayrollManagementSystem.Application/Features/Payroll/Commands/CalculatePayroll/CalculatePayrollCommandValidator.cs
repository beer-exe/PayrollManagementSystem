using FluentValidation;

namespace PayrollManagementSystem.Application.Features.Payroll.Commands.CalculatePayroll
{
    public class CalculatePayrollCommandValidator : AbstractValidator<CalculatePayrollCommand>
    {
        public CalculatePayrollCommandValidator()
        {
            RuleFor(p => p.Thang)
                .InclusiveBetween(1, 12).WithMessage("Tháng không hợp lệ (phải từ 1 đến 12).");

            RuleFor(p => p.Nam)
                .GreaterThan(2000).WithMessage("Năm không hợp lệ (phải lớn hơn 2000).");
        }
    }
}
