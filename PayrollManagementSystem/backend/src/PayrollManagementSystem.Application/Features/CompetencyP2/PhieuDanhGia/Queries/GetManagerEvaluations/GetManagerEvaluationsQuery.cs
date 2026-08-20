using MediatR;
using PayrollManagementSystem.Application.Features.CompetencyP2.PhieuDanhGia.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.PhieuDanhGia.Queries.GetManagerEvaluations
{
    public class GetManagerEvaluationsQuery : IRequest<Response<IEnumerable<PhieuDanhGiaDto>>>
    {
        public Guid TaiKhoanId { get; set; }
        public bool IsHr { get; set; }
    }
}
