using MediatR;
using PayrollManagementSystem.Application.Common.Constants;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.CompetencyP2.KyDanhGia.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.KyDanhGia.Queries.GetKyDanhGias
{
    public class GetKyDanhGiasQuery : IRequest<Response<IEnumerable<KyDanhGiaDto>>>, ICacheableQuery
    {
        public string? CacheKey => CacheKeyConstants.KyDanhGia + "All";
        public TimeSpan? Expiration => null;
    }
}
