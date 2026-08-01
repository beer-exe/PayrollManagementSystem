using MediatR;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.ThueTncn.Commands.UpsertCauHinhGiamTru
{
    public class UpsertCauHinhGiamTruCommand : IRequest<Response<bool>>
    {
        public decimal GiamTruBanThan { get; set; }
        public decimal GiamTruNguoiPhuThuoc { get; set; }
        public string? GhiChu { get; set; }
    }
}
