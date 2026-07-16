using MediatR;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.DonNghi.Commands.CreateMyDonNghi
{
    public class CreateMyDonNghiCommand : IRequest<Response<Guid>>, ITransactionalCommand, ICacheInvalidatorCommand
    {
        public Guid UserId { get; set; }  // Set by controller from JWT
        public string LoaiNghi { get; set; } = null!;
        public DateOnly NgayBatDau { get; set; }
        public DateOnly NgayKetThuc { get; set; }
        public decimal SoNgayNghi { get; set; }
        public string LyDo { get; set; } = null!;
        public string? TaiLieuDinhKem { get; set; }

        public string CacheKeyPrefix => "DonNghi";
    }
}
