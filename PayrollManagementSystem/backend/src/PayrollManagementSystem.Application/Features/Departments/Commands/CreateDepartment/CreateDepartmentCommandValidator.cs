using FluentValidation;

namespace PayrollManagementSystem.Application.Features.Departments.Commands.CreateDepartment
{
    public class CreateDepartmentCommandValidator : AbstractValidator<CreateDepartmentCommand>
    {
        public CreateDepartmentCommandValidator()
        {
            RuleFor(x => x.IdPb)
                .NotEmpty().WithMessage("Mã phòng ban không được để trống.")
                .MaximumLength(50).WithMessage("Mã phòng ban không vượt quá 50 ký tự.");

            RuleFor(x => x.TenPb)
                .NotEmpty().WithMessage("Tên phòng ban không được để trống.")
                .MaximumLength(100).WithMessage("Tên phòng ban không vượt quá 100 ký tự.");
        }
    }
}
