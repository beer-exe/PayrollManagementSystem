using MediatR;
using PayrollManagementSystem.Application.Common.Constants;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.WorkSchedule.Commands.UpdateChiTietLichLamViec
{
    public class UpdateChiTietLichLamViecCommand : IRequest<Response<bool>>, ITransactionalCommand, ICacheInvalidatorCommand
    {
        public Guid IdChiTiet { get; set; }
        public string LoaiNgay { get; set; } = null!;
        public string? TenNgayNghi { get; set; }
        public Guid? IdCaLamViecMacDinh { get; set; }

        public string CacheKeyPrefix => CacheKeyConstants.LichLamViec;
    }
}
