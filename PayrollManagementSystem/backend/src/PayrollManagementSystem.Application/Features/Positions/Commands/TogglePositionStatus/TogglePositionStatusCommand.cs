using MediatR;
using PayrollManagementSystem.Application.Common.Constants;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Positions.Commands.TogglePositionStatus
{
    public class TogglePositionStatusCommand : IRequest<Response<bool>>, ITransactionalCommand, ICacheInvalidatorCommand
    {
        public string IdChucVu { get; set; } = null!;

        public string CacheKeyPrefix => CacheKeyConstants.Positions;
    }
}
