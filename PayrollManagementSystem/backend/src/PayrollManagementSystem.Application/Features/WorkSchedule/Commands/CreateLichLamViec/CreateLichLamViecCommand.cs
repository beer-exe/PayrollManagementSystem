using MediatR;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.WorkSchedule.Commands.CreateLichLamViec
{
    public class CreateLichLamViecCommand : IRequest<Response<Guid>>, ITransactionalCommand, ICacheInvalidatorCommand
    {
        public int Nam { get; set; }
        public string? GhiChu { get; set; }
        public bool UseDefaultShift { get; set; }
        public Guid? DefaultShiftId { get; set; }

        public string CacheKeyPrefix => "LichLamViec_";
    }
}
