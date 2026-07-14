using FluentValidation;

namespace PayrollManagementSystem.Application.Features.DonNghi.Commands.CreateDonNghi
{
    public class CreateDonNghiCommandValidator : AbstractValidator<CreateDonNghiCommand>
    {
        public CreateDonNghiCommandValidator()
        {
            RuleFor(x => x.CccdNhanVien).NotEmpty().WithMessage("CCCD nhân viên không được để trống.");
            RuleFor(x => x.LoaiNghi).NotEmpty().WithMessage("Loại nghỉ không được để trống.");
            RuleFor(x => x.NgayBatDau).NotEmpty().WithMessage("Ngày bắt đầu không được để trống.");
            RuleFor(x => x.NgayKetThuc).NotEmpty()
                .WithMessage("Ngày kết thúc không được để trống.")
                .GreaterThanOrEqualTo(x => x.NgayBatDau)
                .WithMessage("Ngày kết thúc phải sau hoặc bằng ngày bắt đầu.");
            RuleFor(x => x.SoNgayNghi).GreaterThan(0).WithMessage("Số ngày nghỉ phải lớn hơn 0.");
            RuleFor(x => x.LyDo).NotEmpty().WithMessage("Lý do không được để trống.");
        }
    }
}
