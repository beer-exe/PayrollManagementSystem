using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.DonNghi.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.DonNghi.Queries.GetNgayPhepList
{
    public class GetNgayPhepListQuery : IRequest<Response<List<NgayPhepDto>>>, ICacheableQuery
    {
        public int Nam { get; set; }
        public string? IdPhongBan { get; set; }

        public string CacheKey => $"NgayPhep_List_{Nam}_{IdPhongBan ?? "ALL"}";
        public TimeSpan? Expiration => TimeSpan.FromMinutes(10);
    }
}
