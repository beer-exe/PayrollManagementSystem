using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.PhanCongCas.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.PhanCongCas.Queries.GetPhanCongCaByDateRange
{
    public class GetPhanCongCaByDateRangeQueryHandler : IRequestHandler<GetPhanCongCaByDateRangeQuery, Response<IEnumerable<PhanCongCaDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetPhanCongCaByDateRangeQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<IEnumerable<PhanCongCaDto>>> Handle(GetPhanCongCaByDateRangeQuery request, CancellationToken cancellationToken)
        {
            var query = _context.PhanCongCas
                .Include(p => p.NhanVien)
                .Include(p => p.CaLamViec)
                .Where(p => p.NgayLamViec >= request.StartDate && p.NgayLamViec <= request.EndDate && !p.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrEmpty(request.IdPhongBan))
            {
                query = query.Where(p => p.NhanVien.IdPb == request.IdPhongBan);
            }

            var phanCongCas = await query
                .Select(p => new PhanCongCaDto
                {
                    IdPhanCong = p.IdPhanCong,
                    CccdNhanVien = p.CccdNhanVien,
                    HoTenNhanVien = p.NhanVien.HoTen,
                    NgayLamViec = p.NgayLamViec,
                    IdCaLamViec = p.IdCaLamViec,
                    TenCa = p.CaLamViec != null ? p.CaLamViec.TenCa : null,
                    GhiChu = p.GhiChu
                })
                .ToListAsync(cancellationToken);

            return new Response<IEnumerable<PhanCongCaDto>>(phanCongCas, "Lấy danh sách phân công ca thành công.");
        }
    }
}
