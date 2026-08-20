using MediatR;
using PayrollManagementSystem.Application.Common.Constants;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Kpi.Commands.SubmitPhieuKpi
{
    public class SubmitPhieuKpiCommand : IRequest<Response<Guid>>, ITransactionalCommand, ICacheInvalidatorCommand
    {
        public Guid IdPhieuKpi { get; set; }

        public string CacheKeyPrefix => CacheKeyConstants.Kpi;
    }
}
