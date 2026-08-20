using MediatR;
using PayrollManagementSystem.Application.Common.Constants;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Kpi.Commands.CreateKyKpi
{
    public class CreateKyKpiCommand : IRequest<Response<Guid>>, ITransactionalCommand, ICacheInvalidatorCommand
    {
        public string TenKyKpi { get; set; } = string.Empty;
        public int Thang { get; set; }
        public int Nam { get; set; }

        public string CacheKeyPrefix => CacheKeyConstants.Kpi;
    }
}
