using MediatR;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Payroll.Queries.GetMyPayroll
{
    public class GetMyPayrollQuery : IRequest<Response<List<MyPayrollDto>>>, ICacheableQuery
    {
        public int Nam { get; set; }
        
        // Cacheable Query properties
        public string CacheKey => $"GetMyPayroll_{Nam}";
        public TimeSpan? Expiration => TimeSpan.FromMinutes(5);
    }
}
