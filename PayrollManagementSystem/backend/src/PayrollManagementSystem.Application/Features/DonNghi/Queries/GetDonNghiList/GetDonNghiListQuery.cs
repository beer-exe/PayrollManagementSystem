using MediatR;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.DonNghi.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.DonNghi.Queries.GetDonNghiList
{
    public class GetDonNghiListQuery : IRequest<Response<List<DonNghiDto>>>, ICacheableQuery
    {
        public int? Thang { get; set; }
        public int? Nam { get; set; }
        public string? CccdNhanVien { get; set; }
        public string? TrangThai { get; set; }
        public string? IdPhongBan { get; set; }

        public string CacheKey => $"DonNghi_List_{Thang}_{Nam}_{CccdNhanVien ?? "ALL"}_{TrangThai ?? "ALL"}_{IdPhongBan ?? "ALL"}";
        public TimeSpan? Expiration => TimeSpan.FromMinutes(5);
    }
}
