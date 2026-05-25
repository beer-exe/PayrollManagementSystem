using MediatR;
using PayrollManagementSystem.Application.Features.Users.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Users.Queries.GetUsers
{
    public class GetUsersQuery : IRequest<Response<IEnumerable<UserDto>>>
    {
    }
}