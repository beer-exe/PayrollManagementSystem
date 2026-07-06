using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Application.Features.CompetencyP2.KyDanhGia.DTOs;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.KyDanhGia.Queries.GetKyDanhGias
{
    public class GetKyDanhGiasQueryHandler : IRequestHandler<GetKyDanhGiasQuery, Response<IEnumerable<KyDanhGiaDto>>>
    {
        private readonly IApplicationDbContext _context;
        public GetKyDanhGiasQueryHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<IEnumerable<KyDanhGiaDto>>> Handle(GetKyDanhGiasQuery request, CancellationToken cancellationToken)
        {
            var data = await _context.KyDanhGias
                .Select(x => new KyDanhGiaDto
                {
                    IdKyDanhGia = x.IdKyDanhGia,
                    TenKyDanhGia = x.TenKyDanhGia,
                    Nam = x.Nam,
                    NgayBatDau = x.NgayBatDau,
                    NgayKetThuc = x.NgayKetThuc,
                    TrangThai = x.TrangThai
                })
                .OrderByDescending(x => x.NgayBatDau)
                .ToListAsync(cancellationToken);

            return new Response<IEnumerable<KyDanhGiaDto>>(data);
        }
    }
}
