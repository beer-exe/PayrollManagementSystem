using MediatR;
using PayrollManagementSystem.Application.Common.Constants;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.ThueTncn.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.ThueTncn.Queries.GetBacThueList
{
    public class GetBacThueListQuery : IRequest<Response<List<BacThueDto>>>, ICacheableQuery
    {
        public string? CacheKey => CacheKeyConstants.BacThue + "All";
        public TimeSpan? Expiration => TimeSpan.FromHours(24);
    }
}
