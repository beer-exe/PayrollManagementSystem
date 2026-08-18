using MediatR;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Common.Constants;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Kpi.Commands.ConfirmPhieuKpi
{
    public class ConfirmPhieuKpiCommand : IRequest<Response<Guid>>, ITransactionalCommand, ICacheInvalidatorCommand
    {
        public Guid IdPhieuKpi { get; set; }
        public Guid TaiKhoanIdNhanVien { get; set; }

        public string CacheKeyPrefix => CacheKeyConstants.Kpi;
    }
}
