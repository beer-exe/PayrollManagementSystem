using MediatR;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.KhoanKhauTru.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.KhoanKhauTru.Queries.GetKhoanKhauTruList
{
    public class GetKhoanKhauTruListQuery : IRequest<Response<List<KhoanKhauTruDto>>>, ICacheableQuery
    {
        public bool? IsActive { get; set; }

        public string CacheKey => $"KhoanKhauTru_List_{IsActive}";
        public TimeSpan? Expiration => TimeSpan.FromSeconds(300);
    }
}
