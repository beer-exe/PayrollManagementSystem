using FluentValidation;

namespace PayrollManagementSystem.Application.Features.ChamCong.Commands.ImportChamCong
{
    public class ImportChamCongCommandValidator : AbstractValidator<ImportChamCongCommand>
    {
        private static readonly string[] AllowedExtensions = [".csv"];

        public ImportChamCongCommandValidator()
        {
            RuleFor(x => x.FileStream)
                .NotNull().WithMessage("Vui lòng chọn file để import.");

            RuleFor(x => x.FileName)
                .NotEmpty().WithMessage("Tên file không được để trống.")
                .Must(name => !string.IsNullOrEmpty(name)
                    && AllowedExtensions.Contains(Path.GetExtension(name).ToLower()))
                .WithMessage("Chỉ hỗ trợ file .csv.");
        }
    }
}
