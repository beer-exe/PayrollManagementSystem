using FluentValidation;

namespace PayrollManagementSystem.Application.Features.WorkSchedule.Commands.CreateLichLamViec
{
    public class CreateLichLamViecCommandValidator : AbstractValidator<CreateLichLamViecCommand>
    {
        public CreateLichLamViecCommandValidator()
        {
            RuleFor(x => x.Nam)
                .GreaterThanOrEqualTo(2000)
                    .WithMessage("Năm phải lớn hơn hoặc bằng 2000.")
                .LessThanOrEqualTo(2100)
                    .WithMessage("Năm phải nhỏ hơn hoặc bằng 2100.");

            RuleFor(x => x.GhiChu)
                .MaximumLength(500)
                    .WithMessage("Ghi chú không được vượt quá 500 ký tự.");

            RuleFor(x => x.DefaultShiftId)
                .NotNull()
                .When(x => x.UseDefaultShift)
                .WithMessage("Vui lòng chọn ca làm việc mặc định.");
        }
    }
}
