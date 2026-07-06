using MediatR;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Positions.Commands.TogglePositionStatus
{
    public class TogglePositionStatusCommand : IRequest<Response<bool>>
    {
        public string IdChucVu { get; set; } = null!;
    }
}
