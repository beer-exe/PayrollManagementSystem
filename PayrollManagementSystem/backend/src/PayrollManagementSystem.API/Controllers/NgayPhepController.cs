using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollManagementSystem.Application.Features.DonNghi.Commands.UpdateNgayPhep;
using PayrollManagementSystem.Application.Features.DonNghi.Queries.GetNgayPhepList;

namespace PayrollManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/ngay-phep")]
    [Authorize(Roles = "Admin,HR")]
    public class NgayPhepController : ControllerBase
    {
        private readonly IMediator _mediator;
        public NgayPhepController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetList(
            [FromQuery] int nam,
            [FromQuery] string? idPhongBan = null)
        {
            var response = await _mediator.Send(new GetNgayPhepListQuery
            {
                Nam = nam,
                IdPhongBan = idPhongBan,
            });
            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> Update([FromBody] UpdateNgayPhepCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}
