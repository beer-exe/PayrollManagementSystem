using MediatR;
using PayrollManagementSystem.Application.Features.Departments.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Departments.Queries.GetEmployeesByDepartment
{
    public class GetEmployeesByDepartmentQuery : IRequest<Response<IEnumerable<EmployeeInDepartmentDto>>>
    {
        public string IdPb { get; set; } = null!;
    }
}
