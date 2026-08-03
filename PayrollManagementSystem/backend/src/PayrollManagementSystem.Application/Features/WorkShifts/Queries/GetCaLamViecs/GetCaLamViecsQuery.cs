using MediatR;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.WorkShifts.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.WorkShifts.Queries.GetCaLamViecs
{
    public class GetCaLamViecsQuery : IRequest<Response<List<CaLamViecDto>>>, ICacheableQuery
    {
        public bool? TrangThai { get; set; }

        public string CacheKey => $"CaLamViec_All_{TrangThai}";
        public TimeSpan? Expiration => TimeSpan.FromMinutes(30);
    }
}
