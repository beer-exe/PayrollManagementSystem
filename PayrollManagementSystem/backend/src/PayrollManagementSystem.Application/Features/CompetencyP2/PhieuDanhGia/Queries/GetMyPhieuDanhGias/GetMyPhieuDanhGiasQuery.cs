using MediatR;
using PayrollManagementSystem.Application.Features.CompetencyP2.PhieuDanhGia.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.PhieuDanhGia.Queries.GetMyPhieuDanhGias
{
    public class GetMyPhieuDanhGiasQuery : IRequest<Response<IEnumerable<PhieuDanhGiaDto>>>
    {
        public Guid TaiKhoanId { get; set; }
    }
}
