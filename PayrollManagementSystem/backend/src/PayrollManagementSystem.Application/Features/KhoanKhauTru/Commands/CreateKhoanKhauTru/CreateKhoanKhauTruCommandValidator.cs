using FluentValidation;

namespace PayrollManagementSystem.Application.Features.KhoanKhauTru.Commands.CreateKhoanKhauTru
{
    public class CreateKhoanKhauTruCommandValidator : AbstractValidator<CreateKhoanKhauTruCommand>
    {
        public CreateKhoanKhauTruCommandValidator()
        {
            RuleFor(x => x.TenKhoanKhauTru)
                .NotEmpty().WithMessage("Tên khoản khấu trừ không được để trống.")
                .MaximumLength(200).WithMessage("Tên khoản khấu trừ tối đa 200 ký tự.");

            RuleFor(x => x.GiaTri)
                .GreaterThan(0).WithMessage("Giá trị phải lớn hơn 0.");

            RuleFor(x => x.LoaiCongThuc)
                .IsInEnum().WithMessage("Loại công thức không hợp lệ.");
        }
    }
}
