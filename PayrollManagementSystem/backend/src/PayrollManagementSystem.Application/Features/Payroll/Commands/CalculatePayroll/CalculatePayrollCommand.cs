using MediatR;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Application.Common.Constants;

namespace PayrollManagementSystem.Application.Features.Payroll.Commands.CalculatePayroll
{
    public class CalculatePayrollCommand : IRequest<Response<bool>>, ITransactionalCommand, ICacheInvalidatorCommand
    {
        public int Thang { get; set; }
        public int Nam { get; set; }

        public string CacheKeyPrefix => CacheKeyConstants.Payroll;
    }
}
