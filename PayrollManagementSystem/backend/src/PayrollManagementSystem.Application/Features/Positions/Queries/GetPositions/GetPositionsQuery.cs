using MediatR;
using PayrollManagementSystem.Application.Features.Positions.DTOs;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;

using PayrollManagementSystem.Application.Common.Interfaces;

namespace PayrollManagementSystem.Application.Features.Positions.Queries.GetPositions
{
    public class GetPositionsQuery : IRequest<Response<IEnumerable<PositionDto>>>, ICacheableQuery
    {
        public string? SearchTerm { get; set; }
        public TrangThaiChucVu? TrangThai { get; set; }
        public string? IdPhongBan { get; set; }

        public string CacheKey => $"Positions_{SearchTerm ?? "All"}_{TrangThai?.ToString() ?? "All"}_{IdPhongBan ?? "All"}";
        public TimeSpan? Expiration => null;
    }
}
