using MediatR;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Payroll.Commands.CalculatePayroll
{
    public class CalculatePayrollCommand : IRequest<Response<bool>>, ITransactionalCommand
    {
        public int Thang { get; set; }
        public int Nam { get; set; }
    }
}
