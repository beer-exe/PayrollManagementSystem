using FluentValidation;

namespace PayrollManagementSystem.Application.Features.Positions.Commands.UpdatePosition
{
    public class UpdatePositionCommandValidator : AbstractValidator<UpdatePositionCommand>
    {
        public UpdatePositionCommandValidator()
        {
            RuleFor(x => x.IdChucVu)
                .NotEmpty().WithMessage("Mã chức vụ không được để trống.")
                .MaximumLength(50).WithMessage("Mã chức vụ không được vượt quá 50 ký tự.");

            RuleFor(x => x.TenChucVu)
                .NotEmpty().WithMessage("Tên chức vụ không được để trống.")
                .MaximumLength(100).WithMessage("Tên chức vụ không được vượt quá 100 ký tự.");

            RuleFor(x => x.MoTaCongViec)
                .MaximumLength(500).WithMessage("Mô tả công việc không được vượt quá 500 ký tự.")
                .When(x => !string.IsNullOrEmpty(x.MoTaCongViec));
        }
    }
}
