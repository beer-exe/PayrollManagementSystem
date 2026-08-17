using MediatR;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Common.Constants;
using PayrollManagementSystem.Application.Features.Kpi.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Kpi.Commands.AssignPhieuKpi
{
    public class AssignPhieuKpiCommand : IRequest<Response<Guid>>, ITransactionalCommand, ICacheInvalidatorCommand
    {
        public Guid IdPhieuKpi { get; set; }
        public Guid TaiKhoanIdQuanLy { get; set; }
        public List<ChiTietKpiInput> ChiTietKpis { get; set; } = new List<ChiTietKpiInput>();

        public string CacheKeyPrefix => CacheKeyConstants.Kpi;
    }
}
