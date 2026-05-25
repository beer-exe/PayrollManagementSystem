using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollManagementSystem.Application.Features.Users.Commands.CreateUser;
using PayrollManagementSystem.Application.Features.Users.Commands.ResetPassword;
using PayrollManagementSystem.Application.Features.Users.Commands.ToggleUserStatus;
using PayrollManagementSystem.Application.Features.Users.Commands.UpdateUserRole;
using PayrollManagementSystem.Application.Features.Users.Queries.GetRoles;
using PayrollManagementSystem.Application.Features.Users.Queries.GetUsers;

namespace PayrollManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var query = new GetUsersQuery();
            var response = await _mediator.Send(query);
            return Ok(response);
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            var response = await _mediator.Send(new GetRolesQuery());
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPut("{id}/role")]
        public async Task<IActionResult> UpdateUserRole(Guid id, [FromBody] UpdateUserRoleCommand command)
        {
            if (id != command.IdTaiKhoan) return BadRequest("ID không khớp.");
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPut("{id}/toggle-status")]
        public async Task<IActionResult> ToggleUserStatus(Guid id)
        {
            var command = new ToggleUserStatusCommand { IdTaiKhoan = id };
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPut("{id}/reset-password")]
        public async Task<IActionResult> ResetPassword(Guid id, [FromBody] ResetPasswordCommand command)
        {
            if (id != command.IdTaiKhoan) return BadRequest("ID không khớp.");
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}