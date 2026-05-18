using MediatR;
using PayrollManagementSystem.Application.Features.Auth.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommand : IRequest<Response<AuthResponseDto>>
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
    }
}