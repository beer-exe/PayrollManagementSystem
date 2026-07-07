using MediatR;
using PayrollManagementSystem.Application.Features.Positions.DTOs;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Application.Features.Positions.Queries.GetPositions
{
    public class GetPositionsQuery : IRequest<Response<IEnumerable<PositionDto>>>
    {
        public string? SearchTerm { get; set; }
        public TrangThaiChucVu? TrangThai { get; set; }
        public string? IdPhongBan { get; set; }
    }
}
