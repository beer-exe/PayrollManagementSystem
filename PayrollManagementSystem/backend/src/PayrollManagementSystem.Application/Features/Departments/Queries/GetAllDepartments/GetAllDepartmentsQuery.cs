using MediatR;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.Departments.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Departments.Queries.GetAllDepartments
{
    public class GetAllDepartmentsQuery : IRequest<Response<IEnumerable<DepartmentDto>>>, ICacheableQuery
    {
        public string CacheKey => "Departments_All";
        public TimeSpan? Expiration => null; // Use default
    }
}
