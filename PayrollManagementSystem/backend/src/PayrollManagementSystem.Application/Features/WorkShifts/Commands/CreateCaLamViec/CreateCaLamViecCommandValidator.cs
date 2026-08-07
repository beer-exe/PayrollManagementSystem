using FluentValidation;
using System;

namespace PayrollManagementSystem.Application.Features.WorkShifts.Commands.CreateCaLamViec
{
    public class CreateCaLamViecCommandValidator : AbstractValidator<CreateCaLamViecCommand>
    {
        public CreateCaLamViecCommandValidator()
        {
            RuleFor(x => x.TenCa)
                .NotEmpty().WithMessage("Tên ca làm việc không được để trống.")
                .MaximumLength(150).WithMessage("Tên ca không được vượt quá 150 ký tự.");

            RuleFor(x => x.GioBatDau)
                .NotEmpty().WithMessage("Giờ bắt đầu không được để trống.")
                .Must(BeAValidTimeSpan).WithMessage("Giờ bắt đầu không đúng định dạng (HH:mm:ss).");

            RuleFor(x => x.GioKetThuc)
                .NotEmpty().WithMessage("Giờ kết thúc không được để trống.")
                .Must(BeAValidTimeSpan).WithMessage("Giờ kết thúc không đúng định dạng (HH:mm:ss).");

            RuleFor(x => x.HeSoLuong)
                .GreaterThan(0).WithMessage("Hệ số lương phải lớn hơn 0.");

            RuleForEach(x => x.KhungGioNghis).SetValidator(new CreateKhungGioNghiCommandValidator());
        }

        private bool BeAValidTimeSpan(string timeSpanString)
        {
            return TimeSpan.TryParse(timeSpanString, out _);
        }
    }
}
