using MediatR;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.SalarySteps.Commands.DeleteSalaryStep
{
    public class DeleteSalaryStepCommand : IRequest<Response<bool>>
    {
        public string JobGradeId { get; set; } = null!;
        public string StepName { get; set; } = null!;
    }
}
