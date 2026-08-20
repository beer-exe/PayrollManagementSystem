using MediatR;
using PayrollManagementSystem.Application.Common.Constants;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.Kpi.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Kpi.Queries.GetPhieuKpiByNhanVien
{
    public class GetPhieuKpiByNhanVienQuery : IRequest<Response<List<PhieuKpiDto>>>, ICacheableQuery
    {
        public Guid TaiKhoanId { get; set; }

        public string? CacheKey => CacheKeyConstants.Kpi + "PhieuKpiByNhanVien_" + TaiKhoanId;
        public TimeSpan? Expiration => TimeSpan.FromHours(1);
    }
}

