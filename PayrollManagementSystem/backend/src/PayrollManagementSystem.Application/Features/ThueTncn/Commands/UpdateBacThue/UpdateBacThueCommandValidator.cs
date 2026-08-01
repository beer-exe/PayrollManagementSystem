using FluentValidation;

namespace PayrollManagementSystem.Application.Features.ThueTncn.Commands.UpdateBacThue
{
    public class UpdateBacThueCommandValidator : AbstractValidator<UpdateBacThueCommand>
    {
        public UpdateBacThueCommandValidator()
        {
            RuleFor(x => x.IdBacThue)
                .NotEmpty()
                .WithMessage("IdBacThue kh�ng du?c r?ng.");

            RuleFor(x => x.TuGia)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Giới hạn dưới phải >= 0.");

            RuleFor(x => x.ThueSuat)
                .InclusiveBetween(0, 100)
                .WithMessage("Thuế suất phải từ 0 đến 100.");
        }
    }
}
