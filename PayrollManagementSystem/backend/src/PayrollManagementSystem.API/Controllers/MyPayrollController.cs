using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollManagementSystem.Application.Features.Payroll.Queries.GetMyPayroll;

namespace PayrollManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MyPayrollController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MyPayrollController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyPayroll([FromQuery] int nam)
        {
            var query = new GetMyPayrollQuery { Nam = nam };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
