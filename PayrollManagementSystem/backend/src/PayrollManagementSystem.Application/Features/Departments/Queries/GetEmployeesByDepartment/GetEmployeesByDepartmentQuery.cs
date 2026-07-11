using MediatR;
using PayrollManagementSystem.Application.Features.Departments.DTOs;
using PayrollManagementSystem.Application.Wrappers;

using PayrollManagementSystem.Application.Common.Interfaces;

namespace PayrollManagementSystem.Application.Features.Departments.Queries.GetEmployeesByDepartment
{
    public class GetEmployeesByDepartmentQuery : IRequest<Response<IEnumerable<EmployeeInDepartmentDto>>>, ICacheableQuery
    {
        public string IdPb { get; set; } = null!;

        public string CacheKey => $"Departments_{IdPb}_Employees";
        public TimeSpan? Expiration => null; // Use default
    }
}
