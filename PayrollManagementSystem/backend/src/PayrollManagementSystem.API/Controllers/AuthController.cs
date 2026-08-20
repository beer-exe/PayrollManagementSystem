using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PayrollManagementSystem.Application.Features.Auth.Commands.Login;
using PayrollManagementSystem.Application.Features.Auth.Commands.RefreshToken;
using PayrollManagementSystem.Application.Features.Auth.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        [EnableRateLimiting("LoginRateLimit")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            Response<AuthResponseDto>? response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
        {
            Response<AuthResponseDto> response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}
