using MediatR;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Payroll.Queries.GetMyPayroll
{
    public class GetMyPayrollQuery : IRequest<Response<List<MyPayrollDto>>>, ICacheableQuery
    {
        public Guid UserId { get; set; }
        public int Nam { get; set; }
        
        public string CacheKey => $"{PayrollManagementSystem.Application.Common.Constants.CacheKeyConstants.Payroll}My_{UserId}_{Nam}";
        public TimeSpan? Expiration => TimeSpan.FromMinutes(5);
    }
}
