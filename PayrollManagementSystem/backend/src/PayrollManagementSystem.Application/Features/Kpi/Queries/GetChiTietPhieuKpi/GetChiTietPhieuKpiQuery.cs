using PayrollManagementSystem.Application.Common.Interfaces;
using MediatR;
using PayrollManagementSystem.Application.Features.Kpi.DTOs;
using PayrollManagementSystem.Application.Wrappers;

using PayrollManagementSystem.Application.Common.Constants;

namespace PayrollManagementSystem.Application.Features.Kpi.Queries.GetChiTietPhieuKpi
{
    public class GetChiTietPhieuKpiQuery : IRequest<Response<PhieuKpiDetailDto>>, ICacheableQuery
    {
        public Guid IdPhieuKpi { get; set; }

        public string? CacheKey => CacheKeyConstants.Kpi + "ChiTietPhieu_" + IdPhieuKpi;
        public TimeSpan? Expiration => TimeSpan.FromHours(1);
    }
}

