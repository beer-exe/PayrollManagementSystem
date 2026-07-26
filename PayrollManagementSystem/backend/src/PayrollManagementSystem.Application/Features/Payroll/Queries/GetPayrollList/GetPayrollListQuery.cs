using MediatR;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Payroll.Queries.GetPayrollList
{
    public class GetPayrollListQuery : IRequest<Response<List<PayrollListDto>>>
    {
        public int Thang { get; set; }
        public int Nam { get; set; }
    }
}
