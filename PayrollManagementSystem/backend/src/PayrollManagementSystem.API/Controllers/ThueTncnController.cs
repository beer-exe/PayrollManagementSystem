using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollManagementSystem.Application.Features.ThueTncn.Commands.CreateBacThue;
using PayrollManagementSystem.Application.Features.ThueTncn.Commands.DeleteBacThue;
using PayrollManagementSystem.Application.Features.ThueTncn.Commands.UpdateBacThue;
using PayrollManagementSystem.Application.Features.ThueTncn.Commands.UpsertCauHinhGiamTru;
using PayrollManagementSystem.Application.Features.ThueTncn.Queries.GetBacThueList;
using PayrollManagementSystem.Application.Features.ThueTncn.Queries.GetCauHinhGiamTru;

namespace PayrollManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/thue-tncn")]
    [Authorize(Roles = "HR")]
    public class ThueTncnController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ThueTncnController(IMediator mediator) { _mediator = mediator; }

        // ===================== BacThue =====================

        [HttpGet("bac-thue")]
        public async Task<IActionResult> GetBacThueList()
        {
            var result = await _mediator.Send(new GetBacThueListQuery());
            return Ok(result);
        }

        [HttpPost("bac-thue")]
        public async Task<IActionResult> CreateBacThue([FromBody] CreateBacThueCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("bac-thue/{id:guid}")]
        public async Task<IActionResult> UpdateBacThue(Guid id, [FromBody] UpdateBacThueCommand command)
        {
            command.IdBacThue = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("bac-thue/{id:guid}")]
        public async Task<IActionResult> DeleteBacThue(Guid id)
        {
            var result = await _mediator.Send(new DeleteBacThueCommand { IdBacThue = id });
            return Ok(result);
        }

        // ===================== CauHinhGiamTru =====================

        [HttpGet("giam-tru")]
        public async Task<IActionResult> GetCauHinhGiamTru()
        {
            var result = await _mediator.Send(new GetCauHinhGiamTruQuery());
            return Ok(result);
        }

        [HttpPut("giam-tru")]
        public async Task<IActionResult> UpsertCauHinhGiamTru([FromBody] UpsertCauHinhGiamTruCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
