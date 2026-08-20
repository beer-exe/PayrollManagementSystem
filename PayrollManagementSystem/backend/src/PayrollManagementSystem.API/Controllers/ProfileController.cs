using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollManagementSystem.Application.Features.Profile.Commands.UpdateAvatar;
using PayrollManagementSystem.Application.Features.Profile.Queries.GetUserProfile;
using PayrollManagementSystem.Application.Features.Users.Commands.ChangePassword;
using PayrollManagementSystem.Application.Wrappers;
using System.Security.Claims;

namespace PayrollManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProfileController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid userId))
            {
                return Unauthorized(new Response<string>("Phiên đăng nhập không hợp lệ hoặc đã hết hạn."));
            }

            var query = new GetUserProfileQuery { TaiKhoanId = userId };
            var response = await _mediator.Send(query);

            return Ok(response);
        }

        [HttpPut("me/change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPut("me/avatar")]
        public async Task<IActionResult> UpdateAvatar([FromBody] UpdateAvatarCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}