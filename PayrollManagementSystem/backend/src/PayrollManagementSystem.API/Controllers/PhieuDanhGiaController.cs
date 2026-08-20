using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollManagementSystem.Application.Features.CompetencyP2.PhieuDanhGia.Commands.GenerateMyPhieuDanhGia;
using PayrollManagementSystem.Application.Features.CompetencyP2.PhieuDanhGia.Commands.SubmitManagerEvaluation;
using PayrollManagementSystem.Application.Features.CompetencyP2.PhieuDanhGia.Commands.SubmitTuDanhGia;
using PayrollManagementSystem.Application.Features.CompetencyP2.PhieuDanhGia.Queries.GetManagerEvaluations;
using PayrollManagementSystem.Application.Features.CompetencyP2.PhieuDanhGia.Queries.GetMyPhieuDanhGias;
using PayrollManagementSystem.Application.Features.CompetencyP2.PhieuDanhGia.Queries.GetPhieuDanhGiaById;
using System.Security.Claims;

namespace PayrollManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PhieuDanhGiaController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PhieuDanhGiaController(IMediator mediator) => _mediator = mediator;

        [HttpGet("my-evaluations")]
        public async Task<IActionResult> GetMyEvaluations()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid userId)) return Unauthorized();

            var query = new GetMyPhieuDanhGiasQuery { TaiKhoanId = userId };
            return Ok(await _mediator.Send(query));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid userId)) return Unauthorized();

            var query = new GetPhieuDanhGiaByIdQuery
            {
                IdPhieu = id,
                TaiKhoanId = userId,
                IsHr = User.IsInRole("HR")
            };
            return Ok(await _mediator.Send(query));
        }

        [HttpPost("generate")]
        public async Task<IActionResult> Generate([FromBody] GenerateMyPhieuDanhGiaCommand command)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid userId)) return Unauthorized();

            command.TaiKhoanId = userId;
            return Ok(await _mediator.Send(command));
        }

        [HttpPost("submit")]
        public async Task<IActionResult> Submit([FromBody] SubmitTuDanhGiaCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpGet("manager-evaluations")]
        public async Task<IActionResult> GetManagerEvaluations()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid userId)) return Unauthorized();

            var query = new GetManagerEvaluationsQuery
            {
                TaiKhoanId = userId,
                IsHr = User.IsInRole("HR")
            };
            return Ok(await _mediator.Send(query));
        }

        [HttpPost("manager-submit")]
        public async Task<IActionResult> ManagerSubmit([FromBody] SubmitManagerEvaluationCommand command)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid userId)) return Unauthorized();

            command.TaiKhoanId = userId;
            command.IsHr = User.IsInRole("HR");
            return Ok(await _mediator.Send(command));
        }
    }
}
