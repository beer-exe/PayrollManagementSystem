using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollManagementSystem.Application.Features.DonNghi.Commands.CreateMyDonNghi;
using PayrollManagementSystem.Application.Features.DonNghi.Commands.DeleteMyDonNghi;
using PayrollManagementSystem.Application.Features.DonNghi.Queries.GetMyDonNghiList;
using PayrollManagementSystem.Application.Features.DonNghi.Queries.GetMyNgayPhep;
using System.Security.Claims;

namespace PayrollManagementSystem.API.Controllers
{
    /// <summary>
    /// Self-service portal endpoints for all authenticated roles (Admin, HR, Employee).
    /// Each employee can only access their own leave data.
    /// </summary>
    [ApiController]
    [Route("api/don-nghi/me")]
    [Authorize]   // Any authenticated user — no role restriction
    public class DonNghiMyController : ControllerBase
    {
        private readonly IMediator _mediator;
        public DonNghiMyController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetMyList(
            [FromQuery] int? thang,
            [FromQuery] int? nam,
            [FromQuery] string? trangThai = null,
            [FromQuery] string? loaiNghi = null)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var response = await _mediator.Send(new GetMyDonNghiListQuery
            {
                UserId = userId,
                Thang = thang,
                Nam = nam,
                TrangThai = trangThai,
                LoaiNghi = loaiNghi,
            });
            return Ok(response);
        }

        [HttpGet("ngay-phep")]
        public async Task<IActionResult> GetMyNgayPhep([FromQuery] int nam)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var response = await _mediator.Send(new GetMyNgayPhepQuery { UserId = userId, Nam = nam });
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMy([FromBody] CreateMyDonNghiRequest body)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var response = await _mediator.Send(new CreateMyDonNghiCommand
            {
                UserId = userId,
                LoaiNghi = body.LoaiNghi,
                NgayBatDau = body.NgayBatDau,
                NgayKetThuc = body.NgayKetThuc,
                SoNgayNghi = body.SoNgayNghi,
                LyDo = body.LyDo,
                TaiLieuDinhKem = body.TaiLieuDinhKem,
            });
            return Ok(response);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteMy(Guid id)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var response = await _mediator.Send(new DeleteMyDonNghiCommand { Id = id, UserId = userId });
            return Ok(response);
        }
    }

    public record CreateMyDonNghiRequest(
        string LoaiNghi,
        DateOnly NgayBatDau,
        DateOnly NgayKetThuc,
        decimal SoNgayNghi,
        string LyDo,
        string? TaiLieuDinhKem
    );
}
