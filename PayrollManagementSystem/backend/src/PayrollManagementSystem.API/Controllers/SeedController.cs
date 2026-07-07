using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollManagementSystem.Application.Features.SeedData.Commands.SeedDemoData;

namespace PayrollManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SeedController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("demo-data")]
        [AllowAnonymous]
        public async Task<IActionResult> SeedDemoData()
        {
            var command = new SeedDemoDataCommand();
            var result = await _mediator.Send(command);
            
            if (result.Succeeded)
            {
                return Ok(result);
            }
            
            return BadRequest(result);
        }
    }
}
