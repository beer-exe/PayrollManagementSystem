using MediatR;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.SalarySteps.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.SalarySteps.Queries.GetActiveSalarySteps
{
    public class GetActiveSalaryStepsQuery : IRequest<Response<IEnumerable<SalaryStepDto>>>, ICacheableQuery
    {
        public string JobGradeId { get; set; } = null!;

        public string CacheKey => $"SalarySteps_Active_{JobGradeId}";
        public TimeSpan? Expiration => null;
    }
}
