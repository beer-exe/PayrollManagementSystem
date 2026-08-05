using MediatR;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Payroll.Commands.ClosePayroll
{
    public class ClosePayrollCommand : IRequest<Response<bool>>
    {
        public int Thang { get; set; }
        public int Nam { get; set; }
    }
}
