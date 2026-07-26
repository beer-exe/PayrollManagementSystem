using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollManagementSystem.Application.Features.PhanCongCas.Commands.UpsertPhanCongCa;
using PayrollManagementSystem.Application.Features.PhanCongCas.Queries.GetPhanCongCaByDateRange;
using System;
using System.Threading.Tasks;

namespace PayrollManagementSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,HR")]
    public class PhanCongCaController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PhanCongCaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] DateOnly startDate, [FromQuery] DateOnly endDate, [FromQuery] string? idPhongBan)
        {
            var result = await _mediator.Send(new GetPhanCongCaByDateRangeQuery 
            { 
                StartDate = startDate, 
                EndDate = endDate,
                IdPhongBan = idPhongBan
            });
            return Ok(result);
        }

        [HttpPost("upsert")]
        public async Task<IActionResult> Upsert([FromBody] UpsertPhanCongCaCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
