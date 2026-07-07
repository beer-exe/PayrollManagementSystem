using MediatR;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.SeedData.Commands.SeedDemoData
{
    public class SeedDemoDataCommand : IRequest<Response<string>>
    {
    }
}
