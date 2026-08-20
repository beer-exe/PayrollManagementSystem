using MediatR;
using PayrollManagementSystem.Application.Common.Constants;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.PhanCongCas.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.PhanCongCas.Queries.GetPhanCongCaByDateRange
{
    public class GetPhanCongCaByDateRangeQuery : IRequest<Response<IEnumerable<PhanCongCaDto>>>, ICacheableQuery
    {
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string? IdPhongBan { get; set; }

        public string? CacheKey => $"{CacheKeyConstants.PhanCongCa}{StartDate:yyyyMMdd}_{EndDate:yyyyMMdd}_{IdPhongBan ?? "ALL"}";
        public TimeSpan? Expiration => null;
    }
}
