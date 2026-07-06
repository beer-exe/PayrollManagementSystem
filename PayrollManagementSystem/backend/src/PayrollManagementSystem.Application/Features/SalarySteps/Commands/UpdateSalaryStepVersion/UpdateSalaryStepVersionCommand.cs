using MediatR;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.SalarySteps.Commands.UpdateSalaryStepVersion
{
    public class UpdateSalaryStepVersionCommand : IRequest<Response<string>>
    {
        public string PositionId { get; set; } = null!;
        public string StepName { get; set; } = null!;
        public decimal NewP1Salary { get; set; }
        public DateTime NewEffectiveDate { get; set; }
    }
}
