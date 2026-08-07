using MediatR;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.ChamCong.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.ChamCong.Queries.GetMyChamCong
{
    public class GetMyChamCongQuery : IRequest<Response<List<ChamCongDto>>>, ICacheableQuery
    {
        public Guid UserId { get; set; }
        public int Thang { get; set; }
        public int Nam { get; set; }

        public string CacheKey => $"ChamCong_My_{UserId}_{Thang}_{Nam}";
        public TimeSpan? Expiration => TimeSpan.FromMinutes(10);
    }
}
