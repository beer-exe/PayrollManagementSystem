using MediatR;
using PayrollManagementSystem.Application.Features.SalarySteps.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.SalarySteps.Queries.GetActiveSalarySteps
{
    public class GetActiveSalaryStepsQuery : IRequest<Response<IEnumerable<SalaryStepDto>>>
    {
        public string JobGradeId { get; set; } = null!;
    }
}
