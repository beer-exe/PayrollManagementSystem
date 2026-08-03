using MediatR;
using PayrollManagementSystem.Application.Common.Constants;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.WorkSchedule.Commands.DeleteLichLamViec
{
    public class DeleteLichLamViecCommand : IRequest<Response<bool>>, ITransactionalCommand, ICacheInvalidatorCommand
    {
        public Guid IdLich { get; set; }

        public string CacheKeyPrefix => CacheKeyConstants.LichLamViec;
    }
}
