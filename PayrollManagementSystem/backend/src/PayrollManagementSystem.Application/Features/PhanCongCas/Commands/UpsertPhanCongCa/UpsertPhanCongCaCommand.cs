using MediatR;
using PayrollManagementSystem.Application.Common.Constants;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.PhanCongCas.Commands.UpsertPhanCongCa
{
    public class UpsertPhanCongCaCommand : IRequest<Response<bool>>, ICacheInvalidatorCommand
    {
        public string CccdNhanVien { get; set; } = null!;
        public DateOnly NgayLamViec { get; set; }
        public Guid? IdCaLamViec { get; set; }
        public bool XoaPhanCong { get; set; } = false;
        public string? GhiChu { get; set; }

        public string CacheKeyPrefix => CacheKeyConstants.PhanCongCa;
    }
}
