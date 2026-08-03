using FluentValidation;

namespace PayrollManagementSystem.Application.Features.ThueTncn.Commands.CreateBacThue
{
    public class CreateBacThueCommandValidator : AbstractValidator<CreateBacThueCommand>
    {
        public CreateBacThueCommandValidator()
        {
            RuleFor(x => x.Bac)
                .GreaterThan(0)
                .WithMessage("Số bậc phải lớn hơn 0.");

            RuleFor(x => x.TuGia)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Giới hạn dưới phải >= 0.");

            RuleFor(x => x.ThueSuat)
                .InclusiveBetween(0, 100)
                .WithMessage("Thuế suất phải từ 0 đến 100.");
        }
    }
}
