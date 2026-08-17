using MediatR;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Payroll.Queries.GetKyLuongStatus
{
    public class GetKyLuongStatusQuery : IRequest<Response<KyLuongStatusDto>>
    {
        public int Thang { get; set; }
        public int Nam { get; set; }
    }
}
