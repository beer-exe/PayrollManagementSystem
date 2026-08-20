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

            if (!string.IsNullOrWhiteSpace(request.IdPhongBan))
            {
                var daysInMonth = DateTime.DaysInMonth(request.Nam, request.Thang);
                var startOfMonth = new DateOnly(request.Nam, request.Thang, 1);
                var endOfMonth = new DateOnly(request.Nam, request.Thang, daysInMonth);

                var nhanViens = await _context.NhanViens
                    .Where(nv => nv.TrangThai == Domain.Enums.TrangThaiNhanVien.DANG_LAM_VIEC)
                    .ToListAsync(cancellationToken);

                var activeCccds = nhanViens.Select(nv => nv.Cccd).ToList();

                var quyetDinhs = await _context.QuyetDinhNhanSus
                    .Include(qd => qd.ChucVuMoi)
                    .Where(qd => activeCccds.Contains(qd.Cccd)
                              && qd.TrangThai != Domain.Enums.TrangThaiQuyetDinh.HUY_BO
                              && qd.NgayHieuLuc <= endOfMonth
                              && (qd.NgayHetHan == null || qd.NgayHetHan >= startOfMonth))
                    .ToListAsync(cancellationToken);

                var quyetDinhGroup = quyetDinhs
                    .GroupBy(qd => qd.Cccd)
                    .ToDictionary(
                        g => g.Key,
                        g => g.OrderByDescending(qd => qd.NgayHieuLuc).FirstOrDefault()
                    );

                var filteredCccds = new List<string>();
                foreach (var nv in nhanViens)
                {
                    quyetDinhGroup.TryGetValue(nv.Cccd, out var qd);
                    string? actualIdPb = qd?.ChucVuMoi?.IdPhongBan ?? nv.IdPb;

                    if (actualIdPb == request.IdPhongBan)
                    {
                        filteredCccds.Add(nv.Cccd);
                    }
                }

                if (filteredCccds.Count == 0)
                    return new Response<List<ChamCongDto>>(new List<ChamCongDto>());

                query = query.Where(cc => filteredCccds.Contains(cc.CccdNhanVien));
            }

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
