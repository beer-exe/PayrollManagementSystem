using MediatR;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Users.Commands.ToggleUserStatus
{
    public class ToggleUserStatusCommand : IRequest<Response<bool>>
    {
        public Guid IdTaiKhoan { get; set; }
    }
}
