using MediatR;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.WorkSchedule.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.WorkSchedule.Queries.GetLichLamViecs
{
    public class GetLichLamViecsQuery : IRequest<Response<List<LichLamViecDto>>>, ICacheableQuery
    {
        public string CacheKey => "LichLamViec_All";
        public TimeSpan? Expiration => TimeSpan.FromMinutes(30);
    }
}
