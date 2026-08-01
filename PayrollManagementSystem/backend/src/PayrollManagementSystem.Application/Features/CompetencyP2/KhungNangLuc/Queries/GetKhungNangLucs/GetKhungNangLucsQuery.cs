using MediatR;
using PayrollManagementSystem.Application.Common.Constants;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Application.Features.CompetencyP2.KhungNangLuc.DTOs;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.KhungNangLuc.Queries.GetKhungNangLucs
{
    public class GetKhungNangLucsQuery : IRequest<Response<IEnumerable<KhungNangLucDto>>>, ICacheableQuery
    {
        public string IdChucVu { get; set; } = null!;

        public string? CacheKey => $"{CacheKeyConstants.KhungNangLuc}{IdChucVu}";
        public TimeSpan? Expiration => null;
    }
}
