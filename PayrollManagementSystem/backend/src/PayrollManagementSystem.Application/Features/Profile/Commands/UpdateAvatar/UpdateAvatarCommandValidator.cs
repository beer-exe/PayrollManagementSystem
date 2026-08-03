using FluentValidation;

namespace PayrollManagementSystem.Application.Features.Profile.Commands.UpdateAvatar
{
    public class UpdateAvatarCommandValidator : AbstractValidator<UpdateAvatarCommand>
    {
        public UpdateAvatarCommandValidator()
        {
            RuleFor(x => x.AvatarBase64)
                .NotEmpty().WithMessage("Ảnh đại diện không được để trống.");
        }
    }
}
