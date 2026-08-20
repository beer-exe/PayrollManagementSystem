using MediatR;
using PayrollManagementSystem.Application.Common.Constants;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Departments.Commands.CreateDepartment
{
    public class CreateDepartmentCommand : IRequest<Response<string>>, ITransactionalCommand, ICacheInvalidatorCommand
    {
        public string IdPb { get; set; } = null!;
        public string TenPb { get; set; } = null!;

        public string CacheKeyPrefix => CacheKeyConstants.Departments;
    }
}
