using FluentValidation;

namespace PayrollManagementSystem.Application.Features.Payroll.Commands.RequestReviewBangLuong
{
    public class RequestReviewBangLuongCommandValidator : AbstractValidator<RequestReviewBangLuongCommand>
    {
        public RequestReviewBangLuongCommandValidator()
        {
            RuleFor(p => p.IdBangLuong)
                .NotEmpty().WithMessage("{PropertyName} là bắt buộc.");

            RuleFor(p => p.LyDoKhieuNai)
                .NotEmpty().WithMessage("Lý do khiếu nại là bắt buộc.")
                .MaximumLength(500).WithMessage("Lý do khiếu nại không được vượt quá 500 ký tự.");
        }
    }
}
