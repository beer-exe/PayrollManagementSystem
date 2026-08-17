using PayrollManagementSystem.Application.Common.Interfaces;
using MediatR;
using PayrollManagementSystem.Application.Features.Kpi.DTOs;
using PayrollManagementSystem.Application.Wrappers;

using PayrollManagementSystem.Application.Common.Constants;

namespace PayrollManagementSystem.Application.Features.Kpi.Queries.GetPhieuKpiByKyKpi
{
    public class GetPhieuKpiByKyKpiQuery : IRequest<Response<List<PhieuKpiDto>>>, ICacheableQuery
    {
        public Guid IdKyKpi { get; set; }
        public Guid? CurrentUserId { get; set; }

        public string? CacheKey => CacheKeyConstants.Kpi + "PhieuKpiByKy_" + IdKyKpi + "_" + CurrentUserId;
        public TimeSpan? Expiration => TimeSpan.FromHours(1);
    }
}

