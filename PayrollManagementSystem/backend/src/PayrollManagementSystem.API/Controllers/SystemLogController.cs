using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollManagementSystem.Application.Features.SystemManagement.Queries.GetSystemLogs;

namespace PayrollManagementSystem.API.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/system-logs")]
    public class SystemLogController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SystemLogController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetLogs([FromQuery] GetSystemLogsQuery query, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(query, cancellationToken);
            return Ok(response);
        }
    }
}
