using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollManagementSystem.Application.Features.JobGrades.Commands.CreateJobGrade;
using PayrollManagementSystem.Application.Features.JobGrades.Commands.DeleteJobGrade;
using PayrollManagementSystem.Application.Features.JobGrades.Commands.UpdateJobGrade;
using PayrollManagementSystem.Application.Features.JobGrades.Queries.GetJobGrades;

namespace PayrollManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "HR,Admin")]
    public class JobGradesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public JobGradesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetJobGrades()
        {
            var query = new GetJobGradesQuery();
            var response = await _mediator.Send(query);

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateJobGrade([FromBody] CreateJobGradeCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateJobGrade([FromBody] UpdateJobGradeCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpDelete("{idNgachLuong}")]
        public async Task<IActionResult> DeleteJobGrade(string idNgachLuong)
        {
            var command = new DeleteJobGradeCommand { IdNgachLuong = idNgachLuong };
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}
