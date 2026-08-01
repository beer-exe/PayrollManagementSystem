using MediatR;
using PayrollManagementSystem.Application.Common.Constants;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Payroll.Queries.GetPayrollList
{
    public class GetPayrollListQuery : IRequest<Response<List<PayrollListDto>>>, ICacheableQuery
    {
        public int Thang { get; set; }
        public int Nam { get; set; }

        public string? CacheKey => $"{CacheKeyConstants.Payroll}List_{Thang}_{Nam}";
        public TimeSpan? Expiration => null;
    }
}
