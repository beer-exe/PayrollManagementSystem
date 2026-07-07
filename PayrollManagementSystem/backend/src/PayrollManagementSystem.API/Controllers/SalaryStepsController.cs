using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollManagementSystem.Application.Features.SalarySteps.Commands.CreateSalaryStep;
using PayrollManagementSystem.Application.Features.SalarySteps.Commands.DeleteSalaryStep;
using PayrollManagementSystem.Application.Features.SalarySteps.Commands.UpdateSalaryStepVersion;
using PayrollManagementSystem.Application.Features.SalarySteps.Queries.GetActiveSalarySteps;
using PayrollManagementSystem.Application.Features.SalarySteps.Queries.GetSalaryStepHistory;

namespace PayrollManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "HR,Admin")]
    public class SalaryStepsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SalaryStepsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("active/{jobGradeId}")]
        public async Task<IActionResult> GetActiveSalarySteps(string jobGradeId)
        {
            var query = new GetActiveSalaryStepsQuery { JobGradeId = jobGradeId };
            var response = await _mediator.Send(query);

            return Ok(response);
        }

        [HttpGet("history/{jobGradeId}/{stepName}")]
        public async Task<IActionResult> GetSalaryStepHistory(string jobGradeId, string stepName)
        {
            var query = new GetSalaryStepHistoryQuery
            {
                JobGradeId = jobGradeId,
                StepName = Uri.UnescapeDataString(stepName)
            };
            var response = await _mediator.Send(query);

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSalaryStep([FromBody] CreateSalaryStepCommand command)
        {
            var response = await _mediator.Send(command);

            return Ok(response);
        }

        [HttpPost("version")]
        public async Task<IActionResult> UpdateSalaryStepVersion([FromBody] UpdateSalaryStepVersionCommand command)
        {
            var response = await _mediator.Send(command);

            return Ok(response);
        }

        [HttpDelete("{jobGradeId}/{stepName}")]
        public async Task<IActionResult> DeleteSalaryStep(string jobGradeId, string stepName)
        {
            var command = new DeleteSalaryStepCommand
            {
                JobGradeId = jobGradeId,
                StepName = Uri.UnescapeDataString(stepName)
            };
            var response = await _mediator.Send(command);

            return Ok(response);
        }
    }
}
