using MediatR;
using PayrollManagementSystem.Application.Common.Constants;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.Kpi.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Kpi.Queries.GetChiTietPhieuKpi
{
    public class GetChiTietPhieuKpiQuery : IRequest<Response<PhieuKpiDetailDto>>, ICacheableQuery
    {
        public Guid IdPhieuKpi { get; set; }
        public Guid? CurrentUserId { get; set; }

        public string? CacheKey => CacheKeyConstants.Kpi + "ChiTietPhieu_" + IdPhieuKpi + "_" + CurrentUserId;
        public TimeSpan? Expiration => TimeSpan.FromHours(1);
    }
}

