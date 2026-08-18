using FluentValidation;

namespace PayrollManagementSystem.Application.Features.Payroll.Commands.ReopenPayroll
{
    public class ReopenPayrollCommandValidator : AbstractValidator<ReopenPayrollCommand>
    {
        public ReopenPayrollCommandValidator()
        {
            RuleFor(p => p.Thang)
                .NotEmpty().WithMessage("{PropertyName} là bắt buộc.")
                .InclusiveBetween(1, 12).WithMessage("{PropertyName} phải nằm trong khoảng từ 1 đến 12.");

            RuleFor(p => p.Nam)
                .NotEmpty().WithMessage("{PropertyName} là bắt buộc.")
                .GreaterThanOrEqualTo(2000).WithMessage("{PropertyName} không hợp lệ.");

            RuleFor(p => p.LyDo)
                .NotEmpty().WithMessage("Lý do mở lại kỳ lương là bắt buộc.")
                .MaximumLength(500).WithMessage("Lý do không được vượt quá 500 ký tự.");
        }
    }
}
