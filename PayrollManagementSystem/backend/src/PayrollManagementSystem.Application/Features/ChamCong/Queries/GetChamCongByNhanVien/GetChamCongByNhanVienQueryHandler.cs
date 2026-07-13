using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.ChamCong.DTOs;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Extensions;

namespace PayrollManagementSystem.Application.Features.ChamCong.Queries.GetChamCongByNhanVien
{
    public class GetChamCongByNhanVienQueryHandler : IRequestHandler<GetChamCongByNhanVienQuery, Response<List<ChamCongDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetChamCongByNhanVienQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<List<ChamCongDto>>> Handle(GetChamCongByNhanVienQuery request, CancellationToken cancellationToken)
        {
            var query = _context.ChamCongs
                .Include(cc => cc.NhanVien)
                .Where(cc => cc.NgayChamCong.Month == request.Thang
                          && cc.NgayChamCong.Year == request.Nam);

            if (!string.IsNullOrWhiteSpace(request.CccdNhanVien))
                query = query.Where(cc => cc.CccdNhanVien == request.CccdNhanVien);

            var list = await query
                .OrderBy(cc => cc.NgayChamCong)
                .ThenBy(cc => cc.NhanVien.HoTen)
                .ToListAsync(cancellationToken);

            var result = list.Select(cc => new ChamCongDto
            {
                Id = cc.Id,
                CccdNhanVien = cc.CccdNhanVien,
                HoTenNhanVien = cc.NhanVien.HoTen,
                NgayChamCong = cc.NgayChamCong.ToString("yyyy-MM-dd"),
                GioVao = cc.GioVao?.ToString("HH:mm"),
                GioRa = cc.GioRa?.ToString("HH:mm"),
                SoGioLamThucTe = cc.SoGioLamThucTe,
                SoNgayCong = cc.SoNgayCong,
                LoaiNgayCong = cc.LoaiNgayCong.GetDescription(),
                TrangThai = cc.TrangThai.GetDescription(),
                IsNhapTay = cc.IsNhapTay,
                GhiChu = cc.GhiChu,
                NgayTao = cc.CreatedAt.DateTime,
            }).ToList();

            return new Response<List<ChamCongDto>>(result);
        }
    }
}
