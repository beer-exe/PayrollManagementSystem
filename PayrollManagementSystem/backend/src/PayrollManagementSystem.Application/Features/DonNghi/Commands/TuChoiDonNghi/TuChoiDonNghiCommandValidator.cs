using FluentValidation;

namespace PayrollManagementSystem.Application.Features.DonNghi.Commands.TuChoiDonNghi
{
    public class TuChoiDonNghiCommandValidator : AbstractValidator<TuChoiDonNghiCommand>
    {
        public TuChoiDonNghiCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("ID đơn nghỉ không hợp lệ.");
            RuleFor(x => x.LyDoTuChoi).NotEmpty().WithMessage("Lý do từ chối không được để trống.");
        }
    }
}
