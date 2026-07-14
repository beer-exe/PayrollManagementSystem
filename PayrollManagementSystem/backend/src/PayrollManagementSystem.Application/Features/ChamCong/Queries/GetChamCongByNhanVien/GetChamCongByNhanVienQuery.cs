using MediatR;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.ChamCong.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.ChamCong.Queries.GetChamCongByNhanVien
{
    public class GetChamCongByNhanVienQuery : IRequest<Response<List<ChamCongDto>>>, ICacheableQuery
    {
        public string? CccdNhanVien { get; set; }   // null = lấy tất cả nhân viên
        public string? IdPhongBan { get; set; }
        public int Thang { get; set; }
        public int Nam { get; set; }

        public string CacheKey => $"ChamCong_List_{CccdNhanVien ?? "ALL"}_{IdPhongBan ?? "ALL"}_{Thang}_{Nam}";
        public TimeSpan? Expiration => TimeSpan.FromMinutes(10);
    }
}
