using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.Payroll.Queries.GetMyPayroll;

namespace PayrollManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MyPayrollController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public MyPayrollController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyPayroll([FromQuery] int nam)
        {
            if (_currentUserService.UserId == null)
            {
                return Unauthorized();
            }

            var query = new GetMyPayrollQuery { UserId = _currentUserService.UserId.Value, Nam = nam };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
