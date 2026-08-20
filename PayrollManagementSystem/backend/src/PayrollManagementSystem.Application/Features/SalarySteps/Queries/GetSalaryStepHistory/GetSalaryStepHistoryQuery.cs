using MediatR;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.SalarySteps.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.SalarySteps.Queries.GetSalaryStepHistory
{
    public class GetSalaryStepHistoryQuery : IRequest<Response<IEnumerable<SalaryStepDto>>>, ICacheableQuery
    {
        public string JobGradeId { get; set; } = null!;
        public string StepName { get; set; } = null!;

        public string CacheKey => $"SalarySteps_History_{JobGradeId}_{StepName}";
        public TimeSpan? Expiration => null;
    }
}
