using MediatR;
using PayrollManagementSystem.Application.Features.Users.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Users.Queries.GetEmployeesWithoutAccount
{
    public class GetEmployeesWithoutAccountQuery : IRequest<Response<IEnumerable<EmployeeNoAccountDto>>>
    {
    }
}
