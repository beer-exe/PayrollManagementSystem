using MediatR;
using PayrollManagementSystem.Application.Features.SalarySteps.DTOs;
using PayrollManagementSystem.Application.Wrappers;

using PayrollManagementSystem.Application.Common.Interfaces;

namespace PayrollManagementSystem.Application.Features.SalarySteps.Queries.GetActiveSalarySteps
{
    public class GetActiveSalaryStepsQuery : IRequest<Response<IEnumerable<SalaryStepDto>>>, ICacheableQuery
    {
        public string JobGradeId { get; set; } = null!;

        public string CacheKey => $"SalarySteps_Active_{JobGradeId}";
        public TimeSpan? Expiration => null;
    }
}
