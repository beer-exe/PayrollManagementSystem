using MediatR;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Application.Features.CompetencyP2.MucQuyDoi.DTOs;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.MucQuyDoi.Queries.GetMucQuyDois
{
    public class GetMucQuyDoisQuery : IRequest<Response<IEnumerable<MucQuyDoiDto>>>
    {
    }
}
