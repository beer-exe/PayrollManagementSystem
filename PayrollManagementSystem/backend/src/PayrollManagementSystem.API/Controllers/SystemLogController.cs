using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollManagementSystem.Application.Features.SystemManagement.Queries.GetSystemLogs;

namespace PayrollManagementSystem.API.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/system-logs")]
    public class SystemLogController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SystemLogController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetLogs([FromQuery] GetSystemLogsQuery query, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(query, cancellationToken);
            return Ok(response);
        }

        [HttpGet("export/excel")]
        public async Task<IActionResult> ExportExcel([FromQuery] PayrollManagementSystem.Application.Features.SystemManagement.Queries.ExportSystemLogs.ExportSystemLogsQuery query, CancellationToken cancellationToken)
        {
            query.Format = "Excel";
            var fileBytes = await _mediator.Send(query, cancellationToken);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"SystemLogs_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
        }

        [HttpGet("export/pdf")]
        public async Task<IActionResult> ExportPdf([FromQuery] PayrollManagementSystem.Application.Features.SystemManagement.Queries.ExportSystemLogs.ExportSystemLogsQuery query, CancellationToken cancellationToken)
        {
            query.Format = "PDF";
            var fileBytes = await _mediator.Send(query, cancellationToken);
            return File(fileBytes, "application/pdf", $"SystemLogs_{DateTime.Now:yyyyMMddHHmmss}.pdf");
        }
    }
}
