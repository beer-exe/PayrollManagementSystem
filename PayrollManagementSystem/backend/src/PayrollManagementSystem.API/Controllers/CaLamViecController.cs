using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollManagementSystem.Application.Features.WorkShifts.Commands.CreateCaLamViec;
using PayrollManagementSystem.Application.Features.WorkShifts.Commands.DeleteCaLamViec;
using PayrollManagementSystem.Application.Features.WorkShifts.Commands.UpdateCaLamViec;
using PayrollManagementSystem.Application.Features.WorkShifts.Queries.GetCaLamViecs;

namespace PayrollManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Assuming HR or Admin role needed, can specify Roles = "Admin,HR" if needed
    public class CaLamViecController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CaLamViecController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] bool? trangThai)
        {
            var query = new GetCaLamViecsQuery { TrangThai = trangThai };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCaLamViecCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCaLamViecCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("ID không khớp.");
            }
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteCaLamViecCommand { Id = id });
            return Ok(result);
        }
    }
}
