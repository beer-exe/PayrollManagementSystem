using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.KhoanKhauTru.DTOs;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Extensions;

namespace PayrollManagementSystem.Application.Features.KhoanKhauTru.Queries.GetKhoanKhauTruList
{
    public class GetKhoanKhauTruListQueryHandler : IRequestHandler<GetKhoanKhauTruListQuery, Response<List<KhoanKhauTruDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetKhoanKhauTruListQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<List<KhoanKhauTruDto>>> Handle(GetKhoanKhauTruListQuery request, CancellationToken cancellationToken)
        {
            var query = _context.KhoanKhauTrus.AsQueryable();

            if (request.IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == request.IsActive.Value);
            }

            var list = await query
                .OrderBy(x => x.CreatedAt)
                .ToListAsync(cancellationToken);

            var dtos = list.Select(x => new KhoanKhauTruDto
            {
                IdKhoanKhauTru = x.IdKhoanKhauTru,
                TenKhoanKhauTru = x.TenKhoanKhauTru,
                LoaiCongThuc = x.LoaiCongThuc.GetDescription(),
                GiaTri = x.GiaTri,
                GhiChu = x.GhiChu,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt,
            }).ToList();

            return new Response<List<KhoanKhauTruDto>>(dtos);
        }
    }
}
