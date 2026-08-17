using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollManagementSystem.Application.Features.Payroll.Commands.CalculatePayroll;
using PayrollManagementSystem.Application.Features.Payroll.Commands.ClosePayroll;
using PayrollManagementSystem.Application.Features.Payroll.Commands.ReopenPayroll;
using PayrollManagementSystem.Application.Features.Payroll.Queries.GetKyLuongStatus;
using PayrollManagementSystem.Application.Features.Payroll.Queries.GetPayrollList;

namespace PayrollManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PayrollController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PayrollController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("calculate")]
        [Authorize(Roles = "HR,Admin")]
        public async Task<IActionResult> CalculatePayroll([FromBody] CalculatePayrollCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("close")]
        [Authorize(Roles = "HR,Admin")]
        public async Task<IActionResult> ClosePayroll([FromBody] ClosePayrollCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("reopen")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> ReopenPayroll([FromBody] ReopenPayrollCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = "HR,Admin")]
        public async Task<IActionResult> GetPayrollList([FromQuery] int thang, [FromQuery] int nam)
        {
            var query = new GetPayrollListQuery { Thang = thang, Nam = nam };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("status")]
        [Authorize(Roles = "HR,Admin")]
        public async Task<IActionResult> GetKyLuongStatus([FromQuery] int thang, [FromQuery] int nam)
        {
            var query = new GetKyLuongStatusQuery { Thang = thang, Nam = nam };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
