using MediatR;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Users.Commands.UpdateUserRole
{
    public class UpdateUserRoleCommand : IRequest<Response<bool>>
    {
        public Guid IdTaiKhoan { get; set; }
        public Guid IdVaiTroMoi { get; set; }
    }
}
