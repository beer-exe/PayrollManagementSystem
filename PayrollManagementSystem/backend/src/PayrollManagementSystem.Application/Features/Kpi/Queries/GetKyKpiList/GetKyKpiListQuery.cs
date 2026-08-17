using PayrollManagementSystem.Application.Common.Interfaces;
using MediatR;
using PayrollManagementSystem.Application.Features.Kpi.DTOs;
using PayrollManagementSystem.Application.Wrappers;

using PayrollManagementSystem.Application.Common.Constants;

namespace PayrollManagementSystem.Application.Features.Kpi.Queries.GetKyKpiList
{
    public class GetKyKpiListQuery : IRequest<Response<List<KyKpiDto>>>, ICacheableQuery
    {
        public string? CacheKey => CacheKeyConstants.Kpi + "KyKpiList";
        public TimeSpan? Expiration => TimeSpan.FromHours(1);
    }
}

