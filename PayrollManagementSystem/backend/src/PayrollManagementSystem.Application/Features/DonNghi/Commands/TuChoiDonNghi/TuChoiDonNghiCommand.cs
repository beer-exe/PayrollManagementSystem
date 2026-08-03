using MediatR;
using PayrollManagementSystem.Application.Common.Constants;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.DonNghi.Commands.TuChoiDonNghi
{
    public class TuChoiDonNghiCommand : IRequest<Response<bool>>, ITransactionalCommand, ICacheInvalidatorCommand
    {
        public Guid Id { get; set; }
        public string CccdNguoiDuyet { get; set; } = null!;
        public string LyDoTuChoi { get; set; } = null!;
        public string CacheKeyPrefix => CacheKeyConstants.DonNghi;
    }
}
