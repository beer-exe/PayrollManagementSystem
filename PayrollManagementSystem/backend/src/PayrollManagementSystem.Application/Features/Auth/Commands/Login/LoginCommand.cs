using MediatR;
using PayrollManagementSystem.Application.Features.Auth.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Auth.Commands.Login
{
    public class LoginCommand : IRequest<Response<AuthResponseDto>>
    {
        public string TenTaiKhoan { get; set; } = null!;
        public string MatKhau { get; set; } = null!;
    }
}
