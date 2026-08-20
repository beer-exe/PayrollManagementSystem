using MediatR;
using PayrollManagementSystem.Application.Features.ChamCong.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.ChamCong.Queries.GetChamCongSummary
{
    public class GetChamCongSummaryQuery : IRequest<Response<List<ChamCongSummaryDto>>>
    {
        public int Thang { get; set; }
        public int Nam { get; set; }
        public string? IdPhongBan { get; set; }
    }
}
