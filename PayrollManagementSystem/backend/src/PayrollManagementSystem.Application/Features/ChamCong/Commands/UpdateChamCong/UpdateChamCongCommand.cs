using MediatR;
using PayrollManagementSystem.Application.Common.Constants;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.ChamCong.Commands.UpdateChamCong
{
    public class UpdateChamCongCommand : IRequest<Response<bool>>, ITransactionalCommand, ICacheInvalidatorCommand
    {
        public Guid Id { get; set; }
        public TimeOnly? GioVao { get; set; }
        public TimeOnly? GioRa { get; set; }
        public string? GhiChu { get; set; }

        public string CacheKeyPrefix => CacheKeyConstants.ChamCong;
    }
}
