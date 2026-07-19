using FluentValidation;

namespace PayrollManagementSystem.Application.Features.ChamCong.Commands.CreateChamCong
{
    public class CreateChamCongCommandValidator : AbstractValidator<CreateChamCongCommand>
    {
        public CreateChamCongCommandValidator()
        {
            RuleFor(x => x.CccdNhanVien)
                .NotEmpty().WithMessage("CCCD nhân viên không được để trống.")
                .MaximumLength(20).WithMessage("CCCD không được vượt quá 20 ký tự.");

            RuleFor(x => x.NgayChamCong)
                .NotEmpty().WithMessage("Ngày chấm công không được để trống.")
                .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
                    .WithMessage("Ngày chấm công không được lớn hơn ngày hiện tại.");

            RuleFor(x => x.GhiChu)
                .MaximumLength(500).WithMessage("Ghi chú không được vượt quá 500 ký tự.");

            RuleFor(x => x)
                .Must(x => (x.GioVao == null) == (x.GioRa == null))
                .WithMessage("Giờ vào và giờ ra phải cùng được nhập hoặc cùng để trống.")
                .When(x => x.GioVao != null || x.GioRa != null);

            RuleFor(x => x)
                .Must(x => x.GioRa > x.GioVao)
                .WithMessage("Giờ ra phải sau giờ vào.")
                .When(x => x.GioVao != null && x.GioRa != null);
        }
    }
}
