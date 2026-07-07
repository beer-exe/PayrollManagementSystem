using FluentValidation;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.KhungNangLuc.Commands.CreateKhungNangLuc
{
    public class CreateKhungNangLucCommandValidator : AbstractValidator<CreateKhungNangLucCommand>
    {
        public CreateKhungNangLucCommandValidator()
        {
            RuleFor(v => v.TenNangLuc).NotEmpty().WithMessage("Tên năng lực không được để trống.");
            RuleFor(v => v.TyTrong)
                .GreaterThan(0).WithMessage("Tỷ trọng phải lớn hơn 0.")
                .LessThanOrEqualTo(1.0m).WithMessage("Tỷ trọng không được vượt quá 100% (1.0).");
        }
    }
}
