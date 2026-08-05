using MediatR;
using PayrollManagementSystem.Application.Common.Constants;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.ChamCong.Commands.CreateChamCong
{
    public class CreateChamCongCommand : IRequest<Response<Guid>>, ITransactionalCommand, ICacheInvalidatorCommand
    {
        public string CccdNhanVien { get; set; } = null!;
        public DateOnly NgayChamCong { get; set; }
        public TimeOnly? GioVao { get; set; }
        public TimeOnly? GioRa { get; set; }
        public string? GhiChu { get; set; }

        public string CacheKeyPrefix => CacheKeyConstants.ChamCong;
    }
}
