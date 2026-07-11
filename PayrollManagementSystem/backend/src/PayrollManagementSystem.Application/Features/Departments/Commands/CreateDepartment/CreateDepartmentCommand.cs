using MediatR;
using PayrollManagementSystem.Application.Wrappers;

using PayrollManagementSystem.Application.Common.Interfaces;

namespace PayrollManagementSystem.Application.Features.Departments.Commands.CreateDepartment
{
    public class CreateDepartmentCommand : IRequest<Response<string>>, ICacheInvalidatorCommand
    {
        public string IdPb { get; set; } = null!;
        public string TenPb { get; set; } = null!;

        public string CacheKeyPrefix => "Departments_";
    }
}
