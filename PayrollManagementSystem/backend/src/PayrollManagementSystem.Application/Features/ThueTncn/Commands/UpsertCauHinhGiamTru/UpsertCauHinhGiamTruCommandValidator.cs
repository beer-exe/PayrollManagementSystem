using FluentValidation;

namespace PayrollManagementSystem.Application.Features.ThueTncn.Commands.UpsertCauHinhGiamTru
{
    public class UpsertCauHinhGiamTruCommandValidator : AbstractValidator<UpsertCauHinhGiamTruCommand>
    {
        public UpsertCauHinhGiamTruCommandValidator()
        {
            RuleFor(x => x.GiamTruBanThan).GreaterThan(0).WithMessage("M?c gi?m tr? b?n th�n ph?i l?n hon 0.");
            RuleFor(x => x.GiamTruNguoiPhuThuoc).GreaterThan(0).WithMessage("Mức giảm trừ người phụ thuộc phải lớn hơn 0.");
        }
    }
}
