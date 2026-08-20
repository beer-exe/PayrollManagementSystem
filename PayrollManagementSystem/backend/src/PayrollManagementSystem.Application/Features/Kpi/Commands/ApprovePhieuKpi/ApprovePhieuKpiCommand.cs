using MediatR;
using PayrollManagementSystem.Application.Common.Constants;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Kpi.Commands.ApprovePhieuKpi
{
    public class ApprovePhieuKpiCommand : IRequest<Response<Guid>>, ITransactionalCommand, ICacheInvalidatorCommand
    {
        public Guid IdPhieuKpi { get; set; }
        public Guid TaiKhoanIdQuanLy { get; set; }
        public string? NhanXet { get; set; }

        public string CacheKeyPrefix => CacheKeyConstants.Kpi;
    }
}
