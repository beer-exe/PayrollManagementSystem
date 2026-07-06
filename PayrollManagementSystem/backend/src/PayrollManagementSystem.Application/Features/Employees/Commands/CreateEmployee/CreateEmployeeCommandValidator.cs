using FluentValidation;

namespace PayrollManagementSystem.Application.Features.Employees.Commands.CreateEmployee
{
    public class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
    {
        public CreateEmployeeCommandValidator()
        {
            RuleFor(x => x.Cccd)
                .NotEmpty().WithMessage("CCCD không được để trống.")
                .Length(9, 12).WithMessage("CCCD phải từ 9 đến 12 ký tự.");

            RuleFor(x => x.HoTen)
                .NotEmpty().WithMessage("Họ tên không được để trống.")
                .MaximumLength(150).WithMessage("Họ tên không vượt quá 150 ký tự.");

            RuleFor(x => x.Email)
                .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
                .WithMessage("Định dạng Email không hợp lệ.");

            RuleFor(x => x.IdPb)
                .NotEmpty().WithMessage("Vui lòng chọn Phòng ban.");

            RuleFor(x => x.SoHopDong)
                .NotEmpty().WithMessage("Số hợp đồng không được để trống.");

            RuleFor(x => x.LuongCoBan)
                .GreaterThan(0).WithMessage("Lương cơ bản phải lớn hơn 0.");

            RuleFor(x => x.SoQuyetDinh)
                .NotEmpty().WithMessage("Số quyết định không được để trống.");

            RuleFor(x => x.IdChucVu)
                .NotEmpty().WithMessage("Vui lòng chọn Chức vụ.");
        }
    }
}