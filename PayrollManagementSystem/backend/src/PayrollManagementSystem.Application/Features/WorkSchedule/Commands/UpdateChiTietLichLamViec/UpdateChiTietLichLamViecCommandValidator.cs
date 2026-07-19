using FluentValidation;

namespace PayrollManagementSystem.Application.Features.WorkSchedule.Commands.UpdateChiTietLichLamViec
{
    public class UpdateChiTietLichLamViecCommandValidator : AbstractValidator<UpdateChiTietLichLamViecCommand>
    {
        public UpdateChiTietLichLamViecCommandValidator()
        {
            RuleFor(x => x.IdChiTiet)
                .NotEmpty().WithMessage("Id chi tiết không được để trống.");

            RuleFor(x => x.LoaiNgay)
                .NotEmpty().WithMessage("Loại ngày không được để trống.")
                .Must(loaiNgay => PayrollManagementSystem.Domain.Extensions.EnumExtensions.TryGetValueFromDescription<Domain.Enums.LoaiNgay>(loaiNgay, out _))
                .WithMessage("Loại ngày không hợp lệ.");
        }
    }
}
