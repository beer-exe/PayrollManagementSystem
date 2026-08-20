using MediatR;
using PayrollManagementSystem.Application.Common.Constants;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.CompetencyP2.MucQuyDoi.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.MucQuyDoi.Queries.GetMucQuyDois
{
    public class GetMucQuyDoisQuery : IRequest<Response<IEnumerable<MucQuyDoiDto>>>, ICacheableQuery
    {
        public string? CacheKey => CacheKeyConstants.MucQuyDoi + "All";
        public TimeSpan? Expiration => null;
    }
}
