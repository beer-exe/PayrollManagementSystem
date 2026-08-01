using MediatR;
using PayrollManagementSystem.Application.Common.Constants;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Application.Features.KhoanKhauTru.Commands.UpdateKhoanKhauTru
{
    public class UpdateKhoanKhauTruCommand : IRequest<Response<bool>>, ICacheInvalidatorCommand, ITransactionalCommand
    {
        public Guid IdKhoanKhauTru { get; set; }
        public string TenKhoanKhauTru { get; set; } = null!;
        public LoaiCongThucKhauTru LoaiCongThuc { get; set; }
        public decimal GiaTri { get; set; }
        public string? GhiChu { get; set; }
        public bool IsActive { get; set; }

        public string CacheKeyPrefix => CacheKeyConstants.KhoanKhauTru;
    }
}
