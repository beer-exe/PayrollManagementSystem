using MediatR;
using PayrollManagementSystem.Application.Wrappers;

using PayrollManagementSystem.Application.Common.Constants;
using PayrollManagementSystem.Application.Common.Interfaces;

namespace PayrollManagementSystem.Application.Features.Positions.Commands.UpdatePosition
{
    public class UpdatePositionCommand : IRequest<Response<bool>>, ITransactionalCommand, ICacheInvalidatorCommand
    {
        public string IdChucVu { get; set; } = null!;
        public string TenChucVu { get; set; } = null!;
        public string? MoTaCongViec { get; set; }
        public string? IdNgachLuong { get; set; }
        public string IdPhongBan { get; set; } = null!;
        public string? IdChucVuQuanLy { get; set; }

        public string CacheKeyPrefix => CacheKeyConstants.Positions;
    }
}
