using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollManagementSystem.Application.Features.Departments.Commands.AdjustSalary;
using PayrollManagementSystem.Application.Features.Departments.Commands.ChangePosition;
using PayrollManagementSystem.Application.Features.Departments.Commands.CreateDepartment;
using PayrollManagementSystem.Application.Features.Departments.Commands.TransferEmployee;
using PayrollManagementSystem.Application.Features.Departments.Queries.GetAllDepartments;
using PayrollManagementSystem.Application.Features.Departments.Queries.GetEmployeesByDepartment;

namespace PayrollManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,HR")]
    public class DepartmentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DepartmentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetDepartments()
        {
            var query = new GetAllDepartmentsQuery();
            var response = await _mediator.Send(query);
            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> CreateDepartment([FromBody] CreateDepartmentCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("transfer-employee")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> TransferEmployee([FromBody] TransferEmployeeCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpGet("{idPb}/employees")]
        public async Task<IActionResult> GetEmployeesInDepartment(string idPb)
        {
            var query = new GetEmployeesByDepartmentQuery
            {
                IdPb = idPb
            };

            var response = await _mediator.Send(query);
            return Ok(response);
        }

        [HttpPost("adjust-salary")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> AdjustSalary([FromBody] AdjustSalaryCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("change-position")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> ChangePosition([FromBody] ChangePositionCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}
