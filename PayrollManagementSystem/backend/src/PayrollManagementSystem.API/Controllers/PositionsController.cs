using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayrollManagementSystem.Application.Features.Positions.Commands.CreatePosition;
using PayrollManagementSystem.Application.Features.Positions.Commands.TogglePositionStatus;
using PayrollManagementSystem.Application.Features.Positions.Commands.UpdatePosition;
using PayrollManagementSystem.Application.Features.Positions.Queries.GetPositions;

namespace PayrollManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "HR,Admin")]
    public class PositionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PositionsController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetPositions([FromQuery] GetPositionsQuery query)
        {
            return Ok(await _mediator.Send(query));
        }

        [HttpPost]
        public async Task<IActionResult> CreatePosition([FromBody] CreatePositionCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePosition(string id, [FromBody] UpdatePositionCommand command)
        {
            if (id != command.IdChucVu) return BadRequest("Mã chức vụ không khớp.");
            return Ok(await _mediator.Send(command));
        }

        [HttpPut("{id}/toggle-status")]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            return Ok(await _mediator.Send(new TogglePositionStatusCommand { IdChucVu = id }));
        }
    }
}
