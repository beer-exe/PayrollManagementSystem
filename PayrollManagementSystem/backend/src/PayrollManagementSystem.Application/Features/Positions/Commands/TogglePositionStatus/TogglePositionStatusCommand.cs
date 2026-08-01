using MediatR;
using PayrollManagementSystem.Application.Wrappers;

using PayrollManagementSystem.Application.Common.Constants;
using PayrollManagementSystem.Application.Common.Interfaces;

namespace PayrollManagementSystem.Application.Features.Positions.Commands.TogglePositionStatus
{
    public class TogglePositionStatusCommand : IRequest<Response<bool>>, ICacheInvalidatorCommand
    {
        public string IdChucVu { get; set; } = null!;

        public string CacheKeyPrefix => CacheKeyConstants.Positions;
    }
}
