using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollManagementSystem.Application.Features.ChamCong.Commands.CreateChamCong;
using PayrollManagementSystem.Application.Features.ChamCong.Commands.DeleteChamCong;
using PayrollManagementSystem.Application.Features.ChamCong.Commands.ImportChamCong;
using PayrollManagementSystem.Application.Features.ChamCong.Commands.UpdateChamCong;
using PayrollManagementSystem.Application.Features.ChamCong.Queries.GetChamCongByNhanVien;
using PayrollManagementSystem.Application.Features.ChamCong.Queries.GetChamCongSummary;

namespace PayrollManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/cham-cong")]
    [Authorize]
    public class ChamCongController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ChamCongController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> GetList(
            [FromQuery] int thang,
            [FromQuery] int nam,
            [FromQuery] string? cccd = null)
        {
            var response = await _mediator.Send(new GetChamCongByNhanVienQuery
            {
                CccdNhanVien = cccd,
                Thang = thang,
                Nam = nam
            });
            return Ok(response);
        }

        [HttpGet("tong-hop")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> GetSummary(
            [FromQuery] int thang,
            [FromQuery] int nam,
            [FromQuery] string? idPhongBan = null)
        {
            var response = await _mediator.Send(new GetChamCongSummaryQuery
            {
                Thang = thang,
                Nam = nam,
                IdPhongBan = idPhongBan
            });
            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> Create([FromBody] CreateChamCongCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("import")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> Import(IFormFile file)
        {
            var response = await _mediator.Send(new ImportChamCongCommand
            {
                FileStream = file.OpenReadStream(),
                FileName = file.FileName
            });
            return Ok(response);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateChamCongCommand command)
        {
            command.Id = id;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _mediator.Send(new DeleteChamCongCommand { Id = id });
            return Ok(response);
        }
    }
}
