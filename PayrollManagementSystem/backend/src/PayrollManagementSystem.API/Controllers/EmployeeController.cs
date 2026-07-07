using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollManagementSystem.Application.Features.Employees.Commands.ChangeEmployeeStatus;
using PayrollManagementSystem.Application.Features.Employees.Commands.CreateEmployee;
using PayrollManagementSystem.Application.Features.Employees.Commands.UpdateEmployee;
using PayrollManagementSystem.Application.Features.Employees.Queries.GetEmployees;
using PayrollManagementSystem.Application.Wrappers;
using System.IdentityModel.Tokens.Jwt;
using PayrollManagementSystem.Application.Features.Employees.Queries.GetRelations;
using PayrollManagementSystem.Application.Features.Employees.Queries.ExportEmployees;
using PayrollManagementSystem.API.DTOs;

namespace PayrollManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "HR")]
    public class EmployeeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmployeeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees([FromQuery] GetEmployeesQuery query)
        {
            var response = await _mediator.Send(query);
            return Ok(response);
        }

        [HttpGet("relations")]
        public async Task<IActionResult> GetRelations()
        {
            var response = await _mediator.Send(new GetRelationsQuery());
            return Ok(response);
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportEmployees([FromQuery] string? searchTerm, [FromQuery] string? idPb)
        {
            var fileBytes = await _mediator.Send(new ExportEmployeesQuery { SearchTerm = searchTerm, IdPb = idPb });
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DanhSachNhanVien.xlsx");
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPut("{cccd}")]
        public async Task<IActionResult> UpdateEmployee(string cccd, [FromBody] UpdateEmployeeCommand command)
        {
            if (cccd != command.Cccd)
            {
                return BadRequest(new Response<bool>("Mã định danh (CCCD) trên URL không khớp với dữ liệu gửi lên."));
            }

            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPut("{cccd}/status")]
        public async Task<IActionResult> ChangeEmployeeStatus(string cccd, [FromBody] ChangeStatusRequestDto request)
        {
            var changedBy = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName)?.Value
                            ?? User.Identity?.Name
                            ?? "SystemAdmin";

            var command = new ChangeEmployeeStatusCommand
            {
                Cccd = cccd,
                TrangThaiMoi = request.TrangThaiMoi,
                LyDo = request.LyDo,
                NguoiThayDoi = changedBy
            };

            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}
