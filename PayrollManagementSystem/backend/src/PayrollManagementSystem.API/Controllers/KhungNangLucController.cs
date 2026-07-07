using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using PayrollManagementSystem.Application.Features.CompetencyP2.KhungNangLuc.Queries.GetKhungNangLucs;
using PayrollManagementSystem.Application.Features.CompetencyP2.KhungNangLuc.Commands.CreateKhungNangLuc;
using PayrollManagementSystem.Application.Features.CompetencyP2.KhungNangLuc.Commands.UpdateKhungNangLuc;
using PayrollManagementSystem.Application.Features.CompetencyP2.KhungNangLuc.Commands.DeleteKhungNangLuc;

namespace PayrollManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class KhungNangLucController : ControllerBase
    {
        private readonly IMediator _mediator;
        public KhungNangLucController(IMediator mediator) => _mediator = mediator;

        [HttpGet("{idChucVu}")]
        public async Task<IActionResult> Get(string idChucVu)
        {
            return Ok(await _mediator.Send(new GetKhungNangLucsQuery { IdChucVu = idChucVu }));
        }

        [HttpPost]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> Create([FromBody] CreateKhungNangLucCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateKhungNangLucCommand command)
        {
            if (id != command.IdTieuChi) return BadRequest();
            return Ok(await _mediator.Send(command));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return Ok(await _mediator.Send(new DeleteKhungNangLucCommand { IdTieuChi = id }));
        }
    }
}
