using MediatR;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Profile.Commands.UpdateAvatar
{
    public class UpdateAvatarCommand : IRequest<Response<string>>, ITransactionalCommand
    {
        public string AvatarBase64 { get; set; } = null!;
    }
}
