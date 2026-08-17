using MediatR;
using Microsoft.AspNetCore.Mvc;
using PayrollManagementSystem.Application.Features.KyChamCong.Commands.ChotKyChamCong;
using PayrollManagementSystem.Application.Features.KyChamCong.Commands.MoChotKyChamCong;
using PayrollManagementSystem.Application.Features.KyChamCong.Queries.GetKyChamCong;

namespace PayrollManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KyChamCongController : ControllerBase
    {
        private readonly IMediator _mediator;

        public KyChamCongController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{nam}/{thang}")]
        public async Task<IActionResult> GetKyChamCong(int nam, int thang)
        {
            var result = await _mediator.Send(new GetKyChamCongQuery { Nam = nam, Thang = thang });
            return Ok(result);
        }

        [HttpPost("chot-cong")]
        public async Task<IActionResult> ChotCong([FromBody] ChotKyChamCongCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("mo-chot-cong")]
        public async Task<IActionResult> MoChotCong([FromBody] MoChotKyChamCongCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
