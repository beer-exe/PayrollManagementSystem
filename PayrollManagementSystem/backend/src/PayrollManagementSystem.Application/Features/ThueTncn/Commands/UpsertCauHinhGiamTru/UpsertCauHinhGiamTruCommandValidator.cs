using FluentValidation;

namespace PayrollManagementSystem.Application.Features.ThueTncn.Commands.UpsertCauHinhGiamTru
{
    public class UpsertCauHinhGiamTruCommandValidator : AbstractValidator<UpsertCauHinhGiamTruCommand>
    {
        public UpsertCauHinhGiamTruCommandValidator()
        {
            RuleFor(x => x.GiamTruBanThan)
                .GreaterThan(0)
                .WithMessage("Mức giảm trừ phải lớn hơn 0");

            RuleFor(x => x.GiamTruNguoiPhuThuoc)
                .GreaterThan(0)
                .WithMessage("Mức giảm trừ người phụ thuộc phải lớn hơn 0.");
        }
    }
}
