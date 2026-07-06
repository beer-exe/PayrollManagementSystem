using FluentValidation;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.MucQuyDoi.Commands.CreateMucQuyDoi
{
    public class CreateMucQuyDoiCommandValidator : AbstractValidator<CreateMucQuyDoiCommand>
    {
        public CreateMucQuyDoiCommandValidator()
        {
            RuleFor(v => v.XepLoai)
                .NotEmpty().WithMessage("Xếp loại không được để trống.")
                .MaximumLength(50).WithMessage("Xếp loại không vượt quá 50 ký tự.");

            RuleFor(v => v.DiemToiThieu)
                .GreaterThanOrEqualTo(0).WithMessage("Điểm tối thiểu phải lớn hơn hoặc bằng 0.");

            RuleFor(v => v.DiemToiDa)
                .GreaterThan(v => v.DiemToiThieu).WithMessage("Điểm tối đa phải lớn hơn điểm tối thiểu.");

            RuleFor(v => v.HeSoP2)
                .GreaterThanOrEqualTo(0).WithMessage("Hệ số P2 phải lớn hơn hoặc bằng 0.");
        }
    }
}
