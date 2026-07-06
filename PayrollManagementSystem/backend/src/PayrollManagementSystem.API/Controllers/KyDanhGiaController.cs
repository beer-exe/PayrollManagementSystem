using Microsoft.AspNetCore.Mvc;
using MediatR;
using PayrollManagementSystem.Application.Features.CompetencyP2.KyDanhGia.Queries.GetKyDanhGias;
using PayrollManagementSystem.Application.Features.CompetencyP2.KyDanhGia.Commands.CreateKyDanhGia;
using PayrollManagementSystem.Application.Features.CompetencyP2.KyDanhGia.Commands.DeleteKyDanhGia;
using PayrollManagementSystem.Application.Features.CompetencyP2.KyDanhGia.Commands.ChangeStatusKyDanhGia;
using Microsoft.AspNetCore.Authorization;

namespace PayrollManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class KyDanhGiaController : ControllerBase
    {
        private readonly IMediator _mediator;
        public KyDanhGiaController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _mediator.Send(new GetKyDanhGiasQuery()));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> Post([FromBody] CreateKyDanhGiaCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return Ok(await _mediator.Send(new DeleteKyDanhGiaCommand { IdKyDanhGia = id }));
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] ChangeStatusKyDanhGiaCommand command)
        {
            if (id != command.IdKyDanhGia) return BadRequest();
            return Ok(await _mediator.Send(command));
        }
    }
}
