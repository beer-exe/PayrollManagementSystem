using MediatR;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Common.Constants;

namespace PayrollManagementSystem.Application.Features.ThueTncn.Commands.UpsertCauHinhGiamTru
{
    public class UpsertCauHinhGiamTruCommand : IRequest<Response<bool>>, ICacheInvalidatorCommand
    {
        public decimal GiamTruBanThan { get; set; }
        public decimal GiamTruNguoiPhuThuoc { get; set; }
        public string? GhiChu { get; set; }

        public string CacheKeyPrefix => CacheKeyConstants.CauHinhGiamTru;
    }
}
