using MediatR;
using PayrollManagementSystem.Application.Features.Employees.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Employees.Queries.GetEmployees
{
    public class GetEmployeesQuery : IRequest<PagedResponse<IEnumerable<EmployeeDto>>>
    {
        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}