using MediatR;
using PayrollManagementSystem.Application.Common.Constants;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Application.Features.KhoanKhauTru.Commands.CreateKhoanKhauTru
{
    public class CreateKhoanKhauTruCommand : IRequest<Response<Guid>>, ICacheInvalidatorCommand, ITransactionalCommand
    {
        public string TenKhoanKhauTru { get; set; } = null!;
        public LoaiCongThucKhauTru LoaiCongThuc { get; set; }
        public decimal GiaTri { get; set; }
        public string? GhiChu { get; set; }
        public bool IsActive { get; set; } = true;

        public string CacheKeyPrefix => CacheKeyConstants.KhoanKhauTru;
    }
}
