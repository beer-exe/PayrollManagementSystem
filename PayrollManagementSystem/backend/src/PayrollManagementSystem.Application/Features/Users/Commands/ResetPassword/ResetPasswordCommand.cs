using MediatR;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Users.Commands.ResetPassword
{
    public class ResetPasswordCommand : IRequest<Response<bool>>
    {
        public Guid IdTaiKhoan { get; set; }
        public string NewPassword { get; set; } = null!;
    }
}
