using MediatR;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Application.Features.CompetencyP2.PhieuDanhGia.DTOs;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.PhieuDanhGia.Queries.GetPhieuDanhGiaById
{
    public class GetPhieuDanhGiaByIdQuery : IRequest<Response<PhieuDanhGiaDto>>
    {
        public Guid IdPhieu { get; set; }
        public Guid TaiKhoanId { get; set; }
        public bool IsHr { get; set; }
    }
}
