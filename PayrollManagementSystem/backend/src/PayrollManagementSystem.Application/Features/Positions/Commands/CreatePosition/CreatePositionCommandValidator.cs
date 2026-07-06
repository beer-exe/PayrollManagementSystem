using FluentValidation;

namespace PayrollManagementSystem.Application.Features.Positions.Commands.CreatePosition
{
    public class CreatePositionCommandValidator : AbstractValidator<CreatePositionCommand>
    {
        public CreatePositionCommandValidator()
        {
            RuleFor(x => x.IdChucVu).NotEmpty().WithMessage("Mã chức vụ không được để trống.").MaximumLength(50);
            RuleFor(x => x.TenChucVu).NotEmpty().WithMessage("Tên chức vụ không được để trống.").MaximumLength(100);
        }
    }
}
