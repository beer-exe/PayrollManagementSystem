using MediatR;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.SystemManagement.Commands.ClearCache
{
    public class ClearCacheCommand : IRequest<Response<bool>>
    {
    }
}
