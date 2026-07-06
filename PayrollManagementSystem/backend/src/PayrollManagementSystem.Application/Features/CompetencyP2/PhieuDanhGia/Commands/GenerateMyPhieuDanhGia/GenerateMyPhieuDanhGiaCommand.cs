using MediatR;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.PhieuDanhGia.Commands.GenerateMyPhieuDanhGia
{
    public class GenerateMyPhieuDanhGiaCommand : IRequest<Response<Guid>>
    {
        public Guid IdKyDanhGia { get; set; }
        public Guid TaiKhoanId { get; set; }
    }
}
