using MediatR;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Departments.Commands.CreateDepartment
{
    public class CreateDepartmentCommand : IRequest<Response<string>>
    {
        public string IdPb { get; set; } = null!;
        public string TenPb { get; set; } = null!;
    }
}
