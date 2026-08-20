using MediatR;
using PayrollManagementSystem.Application.Common.Constants;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Payroll.Commands.ClosePayroll
{
    public class ClosePayrollCommand : IRequest<Response<bool>>, ICacheInvalidatorCommand
    {
        public int Thang { get; set; }
        public int Nam { get; set; }

        public string CacheKeyPrefix => CacheKeyConstants.Payroll;
    }
}
