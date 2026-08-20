using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollManagementSystem.Application.Features.HrDecisions.Queries.GetNextDecisionCode;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class QuyetDinhNhanSuController : ControllerBase
    {
        private readonly IMediator _mediator;

        public QuyetDinhNhanSuController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("generate-code")]
        public async Task<IActionResult> GenerateCode([FromQuery] string type)
        {
            var query = new GetNextDecisionCodeQuery { Type = type };
            var result = await _mediator.Send(query);
            return Ok(new Response<string> { Data = result, Succeeded = true });
        }
    }
}
