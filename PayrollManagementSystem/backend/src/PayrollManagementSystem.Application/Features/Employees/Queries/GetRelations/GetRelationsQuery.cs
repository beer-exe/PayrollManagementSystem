using MediatR;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Employees.Queries.GetRelations
{
    public class GetRelationsQuery : IRequest<Response<IEnumerable<RelationDto>>>
    {
    }
}
