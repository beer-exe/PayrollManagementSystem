using FluentValidation;
using System;

namespace PayrollManagementSystem.Application.Features.WorkShifts.Commands.UpdateCaLamViec
{
    public class UpdateKhungGioNghiCommandValidator : AbstractValidator<UpdateKhungGioNghiCommand>
    {
        public UpdateKhungGioNghiCommandValidator()
        {
            RuleFor(x => x.TenKhoangNghi)
                .NotEmpty().WithMessage("Tên khoảng nghỉ không được để trống.")
                .MaximumLength(150).WithMessage("Tên khoảng nghỉ không được vượt quá 150 ký tự.");

            RuleFor(x => x.GioBatDau)
                .NotEmpty().WithMessage("Giờ bắt đầu khoảng nghỉ không được để trống.")
                .Must(BeAValidTimeSpan).WithMessage("Giờ bắt đầu khoảng nghỉ không đúng định dạng (HH:mm:ss).");

            RuleFor(x => x.GioKetThuc)
                .NotEmpty().WithMessage("Giờ kết thúc khoảng nghỉ không được để trống.")
                .Must(BeAValidTimeSpan).WithMessage("Giờ kết thúc khoảng nghỉ không đúng định dạng (HH:mm:ss).");
        }

        private bool BeAValidTimeSpan(string timeSpanString)
        {
            return TimeSpan.TryParse(timeSpanString, out _);
        }
    }
}
