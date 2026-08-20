using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollManagementSystem.Application.Features.WorkSchedule.Queries.GetMySchedule;
using System.Security.Claims;

namespace PayrollManagementSystem.API.Controllers
{
    /// <summary>
    /// Self-service portal endpoints for all authenticated roles (Admin, HR, Employee).
    /// Each employee can only access their own work schedule data.
    /// </summary>
    [ApiController]
    [Route("api/lich-lam-viec/me")]
    [Authorize]   // Any authenticated user — no role restriction
    public class LichLamViecMyController : ControllerBase
    {
        private readonly IMediator _mediator;
        public LichLamViecMyController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetMySchedule([FromQuery] int thang, [FromQuery] int nam)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var response = await _mediator.Send(new GetMyScheduleQuery
            {
                UserId = userId,
                Thang = thang,
                Nam = nam
            });
            return Ok(response);
        }
    }
}
