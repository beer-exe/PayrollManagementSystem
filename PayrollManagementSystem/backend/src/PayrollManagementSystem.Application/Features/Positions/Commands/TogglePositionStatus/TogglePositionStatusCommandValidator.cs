using FluentValidation;

namespace PayrollManagementSystem.Application.Features.Positions.Commands.TogglePositionStatus
{
    public class TogglePositionStatusCommandValidator : AbstractValidator<TogglePositionStatusCommand>
    {
        public TogglePositionStatusCommandValidator()
        {
            RuleFor(x => x.IdChucVu)
                .NotEmpty().WithMessage("Mã chức vụ không được để trống.");
        }
    }
}
