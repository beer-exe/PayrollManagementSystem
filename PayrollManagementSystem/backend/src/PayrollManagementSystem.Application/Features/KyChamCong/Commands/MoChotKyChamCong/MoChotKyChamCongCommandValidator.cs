using FluentValidation;

namespace PayrollManagementSystem.Application.Features.KyChamCong.Commands.MoChotKyChamCong
{
    public class MoChotKyChamCongCommandValidator : AbstractValidator<MoChotKyChamCongCommand>
    {
        public MoChotKyChamCongCommandValidator()
        {
            RuleFor(p => p.Thang)
                .NotEmpty().WithMessage("{PropertyName} là bắt buộc.")
                .InclusiveBetween(1, 12).WithMessage("{PropertyName} phải nằm trong khoảng từ 1 đến 12.");

            RuleFor(p => p.Nam)
                .NotEmpty().WithMessage("{PropertyName} là bắt buộc.")
                .GreaterThanOrEqualTo(2000).WithMessage("{PropertyName} không hợp lệ.");
        }
    }
}