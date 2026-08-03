using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollManagementSystem.Application.Features.WorkSchedule.Commands.CreateLichLamViec;
using PayrollManagementSystem.Application.Features.WorkSchedule.Commands.DeleteLichLamViec;
using PayrollManagementSystem.Application.Features.WorkSchedule.Commands.UpdateChiTietLichLamViec;
using PayrollManagementSystem.Application.Features.WorkSchedule.Queries.GetChiTietLichLamViec;
using PayrollManagementSystem.Application.Features.WorkSchedule.Queries.GetLichLamViecs;

namespace PayrollManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/lich-lam-viec")]
    [Authorize]
    public class LichLamViecController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LichLamViecController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var response = await _mediator.Send(new GetLichLamViecsQuery());
            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> Create([FromBody] CreateLichLamViecCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _mediator.Send(new DeleteLichLamViecCommand { IdLich = id });
            return Ok(response);
        }

        [HttpGet("{id:guid}/chi-tiet")]
        public async Task<IActionResult> GetChiTiet(
            Guid id,
            [FromQuery] int thang = 1,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 31)
        {
            var response = await _mediator.Send(new GetChiTietLichLamViecQuery
            {
                IdLich = id,
                Thang = thang,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
            return Ok(response);
        }

        [HttpPut("chi-tiet")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> UpdateChiTiet([FromBody] UpdateChiTietLichLamViecCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}
