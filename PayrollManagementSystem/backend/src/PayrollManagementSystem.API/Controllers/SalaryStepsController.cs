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

        [HttpGet("active/{positionId}")]
        public async Task<IActionResult> GetActiveSalarySteps(string positionId)
        {
            var query = new GetActiveSalaryStepsQuery { PositionId = positionId };
            var response = await _mediator.Send(query);

            return Ok(response);
        }

        [HttpGet("history/{positionId}/{stepName}")]
        public async Task<IActionResult> GetSalaryStepHistory(string positionId, string stepName)
        {
            var query = new GetSalaryStepHistoryQuery
            {
                PositionId = positionId,
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

        [HttpDelete("{positionId}/{stepName}")]
        public async Task<IActionResult> DeleteSalaryStep(string positionId, string stepName)
        {
            var command = new DeleteSalaryStepCommand
            {
                PositionId = positionId,
                StepName = Uri.UnescapeDataString(stepName)
            };
            var response = await _mediator.Send(command);

            return Ok(response);
        }
    }
}
