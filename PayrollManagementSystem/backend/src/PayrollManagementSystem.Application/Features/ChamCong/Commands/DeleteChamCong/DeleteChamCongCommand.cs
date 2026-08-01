using MediatR;
using PayrollManagementSystem.Application.Common.Constants;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.ChamCong.Commands.DeleteChamCong
{
    public class DeleteChamCongCommand : IRequest<Response<bool>>, ITransactionalCommand, ICacheInvalidatorCommand
    {
        public Guid Id { get; set; }

        public string CacheKeyPrefix => CacheKeyConstants.ChamCong;
    }
}
