using MediatR;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Application.Features.CompetencyP2.PhieuDanhGia.DTOs;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.PhieuDanhGia.Queries.GetMyPhieuDanhGias
{
    public class GetMyPhieuDanhGiasQuery : IRequest<Response<IEnumerable<PhieuDanhGiaDto>>>
    {
        public Guid TaiKhoanId { get; set; }
    }
}
