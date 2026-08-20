using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollManagementSystem.Application.Features.CompetencyP2.MucQuyDoi.Commands.CreateMucQuyDoi;
using PayrollManagementSystem.Application.Features.CompetencyP2.MucQuyDoi.Commands.DeleteMucQuyDoi;
using PayrollManagementSystem.Application.Features.CompetencyP2.MucQuyDoi.Commands.UpdateMucQuyDoi;
using PayrollManagementSystem.Application.Features.CompetencyP2.MucQuyDoi.Queries.GetMucQuyDois;

namespace PayrollManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MucQuyDoiP2Controller : ControllerBase
    {
        private readonly IMediator _mediator;
        public MucQuyDoiP2Controller(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _mediator.Send(new GetMucQuyDoisQuery()));
        }

        [HttpPost]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> Post([FromBody] CreateMucQuyDoiCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> Put(Guid id, [FromBody] UpdateMucQuyDoiCommand command)
        {
            if (command.IdQuyDoi == Guid.Empty)
            {
                command.IdQuyDoi = id;
            }

            if (id != command.IdQuyDoi)
            {
                return BadRequest("ID không khớp.");
            }
            return Ok(await _mediator.Send(command));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return Ok(await _mediator.Send(new DeleteMucQuyDoiCommand(id)));
        }
    }
}
