using MediatR;
using PayrollManagementSystem.Application.Wrappers;
using System.Collections.Generic;

namespace PayrollManagementSystem.Application.Features.Employees.Queries.GetRelations
{
    public class GetRelationsQuery : IRequest<Response<IEnumerable<RelationDto>>>
    {
    }
}
