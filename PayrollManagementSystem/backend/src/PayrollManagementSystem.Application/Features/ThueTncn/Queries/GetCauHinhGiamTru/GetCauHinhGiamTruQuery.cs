using MediatR;
using PayrollManagementSystem.Application.Common.Constants;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Application.Features.ThueTncn.DTOs;

namespace PayrollManagementSystem.Application.Features.ThueTncn.Queries.GetCauHinhGiamTru
{
    public class GetCauHinhGiamTruQuery : IRequest<Response<CauHinhGiamTruDto>>, ICacheableQuery
    {
        public string? CacheKey => CacheKeyConstants.CauHinhGiamTru + "All";
        public TimeSpan? Expiration => TimeSpan.FromHours(24);
    }
}
