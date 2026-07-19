using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollManagementSystem.Application.Features.DonNghi.Commands.CreateDonNghi;
using PayrollManagementSystem.Application.Features.DonNghi.Commands.DeleteDonNghi;
using PayrollManagementSystem.Application.Features.DonNghi.Commands.DuyetDonNghi;
using PayrollManagementSystem.Application.Features.DonNghi.Commands.TuChoiDonNghi;
using PayrollManagementSystem.Application.Features.DonNghi.Queries.GetDonNghiList;
using System.Security.Claims;

namespace PayrollManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/don-nghi")]
    [Authorize(Roles = "Admin,HR")]
    public class DonNghiController : ControllerBase
    {
        private readonly IMediator _mediator;
        public DonNghiController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetList(
            [FromQuery] int? thang,
            [FromQuery] int? nam,
            [FromQuery] string? cccd = null,
            [FromQuery] string? trangThai = null,
            [FromQuery] string? idPhongBan = null)
        {
            var response = await _mediator.Send(new GetDonNghiListQuery
            {
                Thang = thang,
                Nam = nam,
                CccdNhanVien = cccd,
                TrangThai = trangThai,
                IdPhongBan = idPhongBan,
            });
            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> Create([FromBody] CreateDonNghiCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPatch("{id:guid}/duyet")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> Duyet(Guid id)
        {
            var cccdNguoiDuyet = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            var response = await _mediator.Send(new DuyetDonNghiCommand
            {
                Id = id,
                CccdNguoiDuyet = cccdNguoiDuyet,
            });
            return Ok(response);
        }

        [HttpPatch("{id:guid}/tu-choi")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> TuChoi(Guid id, [FromBody] TuChoiRequest request)
        {
            var cccdNguoiDuyet = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            var response = await _mediator.Send(new TuChoiDonNghiCommand
            {
                Id = id,
                CccdNguoiDuyet = cccdNguoiDuyet,
                LyDoTuChoi = request.LyDoTuChoi,
            });
            return Ok(response);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _mediator.Send(new DeleteDonNghiCommand { Id = id });
            return Ok(response);
        }
    }

    public record TuChoiRequest(string LyDoTuChoi);
}
