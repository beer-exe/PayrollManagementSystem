using FluentValidation;

namespace PayrollManagementSystem.Application.Features.Employees.Commands.UpdateEmployee
{
    public class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
    {
        public UpdateEmployeeCommandValidator()
        {
            RuleFor(x => x.Cccd)
                .NotEmpty().WithMessage("Mã định danh CCCD không được để trống.");

            RuleFor(x => x.HoTen)
                .NotEmpty().WithMessage("Họ tên nhân viên không được để trống.")
                .MaximumLength(150).WithMessage("Họ tên không được vượt quá 150 ký tự.");

            RuleFor(x => x.Sdt)
                .MaximumLength(15).WithMessage("Số điện thoại không được vượt quá 15 ký tự.");

            RuleFor(x => x.Email)
                .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email)).WithMessage("Định dạng Email không hợp lệ.")
                .MaximumLength(100).WithMessage("Email không được vượt quá 100 ký tự.");
        }
    }
}