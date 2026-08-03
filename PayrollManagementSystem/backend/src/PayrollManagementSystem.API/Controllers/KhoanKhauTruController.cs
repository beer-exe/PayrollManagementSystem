using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollManagementSystem.Application.Features.KhoanKhauTru.Commands.CreateKhoanKhauTru;
using PayrollManagementSystem.Application.Features.KhoanKhauTru.Commands.DeleteKhoanKhauTru;
using PayrollManagementSystem.Application.Features.KhoanKhauTru.Commands.UpdateKhoanKhauTru;
using PayrollManagementSystem.Application.Features.KhoanKhauTru.Queries.GetKhoanKhauTruList;

namespace PayrollManagementSystem.API.Controllers
{
    [Route("api/khoan-khau-tru")]
    [ApiController]
    [Authorize(Roles = "HR")]
    public class KhoanKhauTruController : ControllerBase
    {
        private readonly IMediator _mediator;

        public KhoanKhauTruController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] bool? isActive)
        {
            var query = new GetKhoanKhauTruListQuery { IsActive = isActive };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateKhoanKhauTruCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateKhoanKhauTruCommand command)
        {
            command.IdKhoanKhauTru = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var command = new DeleteKhoanKhauTruCommand { IdKhoanKhauTru = id };
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
