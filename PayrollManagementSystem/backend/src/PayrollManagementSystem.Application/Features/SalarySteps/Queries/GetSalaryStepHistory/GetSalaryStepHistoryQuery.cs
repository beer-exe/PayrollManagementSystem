using MediatR;
using PayrollManagementSystem.Application.Features.SalarySteps.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.SalarySteps.Queries.GetSalaryStepHistory
{
    public class GetSalaryStepHistoryQuery : IRequest<Response<IEnumerable<SalaryStepDto>>>
    {
        public string PositionId { get; set; } = null!;
        public string StepName { get; set; } = null!;
    }
}
