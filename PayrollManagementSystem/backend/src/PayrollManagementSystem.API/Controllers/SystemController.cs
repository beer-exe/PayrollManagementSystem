using MediatR;
using Microsoft.AspNetCore.Mvc;
using PayrollManagementSystem.Application.Features.SystemManagement.Commands.ClearCache;

namespace PayrollManagementSystem.API.Controllers
{
    [ApiController]
    // [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    public class SystemController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SystemController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("clear-cache")]
        public async Task<IActionResult> ClearCache(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new ClearCacheCommand(), cancellationToken);
            return Ok(response);
        }
    }
}
