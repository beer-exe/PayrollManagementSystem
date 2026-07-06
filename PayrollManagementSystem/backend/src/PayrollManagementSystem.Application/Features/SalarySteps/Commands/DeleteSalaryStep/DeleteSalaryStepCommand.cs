using MediatR;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.SalarySteps.Commands.DeleteSalaryStep
{
    public class DeleteSalaryStepCommand : IRequest<Response<bool>>
    {
        public string PositionId { get; set; } = null!;
        public string StepName { get; set; } = null!;
    }
}
