using FluentValidation;

namespace PayrollManagementSystem.Application.Features.ChamCong.Commands.UpdateChamCong
{
    public class UpdateChamCongCommandValidator : AbstractValidator<UpdateChamCongCommand>
    {
        public UpdateChamCongCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("ID bản ghi chấm công không được để trống.");

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
