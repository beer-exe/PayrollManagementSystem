using MediatR;
using PayrollManagementSystem.Application.Features.Users.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Users.Queries.GetRoles
{
    public class GetRolesQuery : IRequest<Response<IEnumerable<RoleDto>>> { }
}