using MediatR;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.ChamCong.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.ChamCong.Queries.GetChamCongSummary
{
    public class GetChamCongSummaryQuery : IRequest<Response<List<ChamCongSummaryDto>>>, ICacheableQuery
    {
        public int Thang { get; set; }
        public int Nam { get; set; }
        public string? IdPhongBan { get; set; }

        public string CacheKey => $"ChamCong_Summary_{Thang}_{Nam}_{IdPhongBan ?? "ALL"}";
        public TimeSpan? Expiration => TimeSpan.FromMinutes(15);
    }
}
