using MediatR;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.WorkSchedule.Commands.DeleteLichLamViec
{
    public class DeleteLichLamViecCommand : IRequest<Response<bool>>, ICacheInvalidatorCommand
    {
        public Guid IdLich { get; set; }

        public string CacheKeyPrefix => "LichLamViec_";
    }
}
