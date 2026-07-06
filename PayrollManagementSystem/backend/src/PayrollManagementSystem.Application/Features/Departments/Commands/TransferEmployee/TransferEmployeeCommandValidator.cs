using FluentValidation;

namespace PayrollManagementSystem.Application.Features.Departments.Commands.TransferEmployee
{
    public class TransferEmployeeCommandValidator : AbstractValidator<TransferEmployeeCommand>
    {
        public TransferEmployeeCommandValidator()
        {
            RuleFor(x => x.SoQuyetDinh).NotEmpty().WithMessage("Số quyết định không được để trống.");
            RuleFor(x => x.Cccd).NotEmpty().WithMessage("Mã định danh (CCCD) không được để trống.");
            RuleFor(x => x.IdPbMoi).NotEmpty().WithMessage("Mã phòng ban mới không được để trống.");
            RuleFor(x => x.IdChucVuMoi).NotEmpty().WithMessage("Mã chức vụ mới không được để trống.");
        }
    }
}
