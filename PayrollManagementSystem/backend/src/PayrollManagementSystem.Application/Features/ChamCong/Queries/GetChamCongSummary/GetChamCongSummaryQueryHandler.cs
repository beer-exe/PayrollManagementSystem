using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.ChamCong.DTOs;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Application.Features.ChamCong.Queries.GetChamCongSummary
{
    public class GetChamCongSummaryQueryHandler : IRequestHandler<GetChamCongSummaryQuery, Response<List<ChamCongSummaryDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetChamCongSummaryQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<List<ChamCongSummaryDto>>> Handle(GetChamCongSummaryQuery request, CancellationToken cancellationToken)
        {
            var chiTietLichs = await _context.ChiTietLichLamViecs
                .Include(ct => ct.CaLamViecMacDinh)
                    .ThenInclude(c => c.KhungGioNghis)
                .Where(ct => ct.Ngay.Month == request.Thang
                          && ct.Ngay.Year == request.Nam)
                .ToDictionaryAsync(ct => ct.Ngay.Day, cancellationToken);

            var nvQuery = _context.NhanViens
                .Include(nv => nv.PhongBan)
                .Where(nv => nv.TrangThai == TrangThaiNhanVien.DANG_LAM_VIEC);

            var nhanViens = await nvQuery.ToListAsync(cancellationToken);
            var allActiveCccd = nhanViens.Select(nv => nv.Cccd).ToList();

            var daysInMonth = DateTime.DaysInMonth(request.Nam, request.Thang);
            var startOfMonth = new DateOnly(request.Nam, request.Thang, 1);
            var endOfMonth = new DateOnly(request.Nam, request.Thang, daysInMonth);

            var quyetDinhs = await _context.QuyetDinhNhanSus
                .Include(qd => qd.ChucVuMoi)
                    .ThenInclude(cv => cv.PhongBan)
                .Where(qd => allActiveCccd.Contains(qd.Cccd) 
                          && qd.TrangThai != TrangThaiQuyetDinh.HUY_BO
                          && qd.NgayHieuLuc <= endOfMonth
                          && (qd.NgayHetHan == null || qd.NgayHetHan >= startOfMonth))
                .ToListAsync(cancellationToken);
                
            var quyetDinhGroup = quyetDinhs
                .GroupBy(qd => qd.Cccd)
                .ToDictionary(
                    g => g.Key, 
                    g => g.OrderByDescending(qd => qd.NgayHieuLuc).FirstOrDefault()
                );

            // Resolve actual department and filter
            var filteredNhanViens = new List<Domain.Models.NhanVien>();
            foreach (var nv in nhanViens)
            {
                quyetDinhGroup.TryGetValue(nv.Cccd, out var qd);
                string? actualIdPb = qd?.ChucVuMoi?.IdPhongBan ?? nv.IdPb;

                if (string.IsNullOrWhiteSpace(request.IdPhongBan) || actualIdPb == request.IdPhongBan)
                {
                    filteredNhanViens.Add(nv);
                }
            }

            var allFilteredCccd = filteredNhanViens.Select(nv => nv.Cccd).ToList();

            if (allFilteredCccd.Count == 0)
                return new Response<List<ChamCongSummaryDto>>(new List<ChamCongSummaryDto>());

            var chamCongs = await _context.ChamCongs
                .Where(cc => allFilteredCccd.Contains(cc.CccdNhanVien)
                          && cc.NgayChamCong.Month == request.Thang
                          && cc.NgayChamCong.Year == request.Nam)
                .ToListAsync(cancellationToken);

            var chamCongGroup = chamCongs
                .GroupBy(cc => cc.CccdNhanVien)
                .ToDictionary(g => g.Key, g => g.ToList());

            var phanCongCas = await _context.PhanCongCas
                .Include(p => p.CaLamViec)
                    .ThenInclude(c => c.KhungGioNghis)
                .Where(p => allFilteredCccd.Contains(p.CccdNhanVien) 
                         && p.NgayLamViec.Month == request.Thang 
                         && p.NgayLamViec.Year == request.Nam)
                .ToListAsync(cancellationToken);

            var phanCongGroup = phanCongCas
                .GroupBy(p => p.CccdNhanVien)
                .ToDictionary(g => g.Key, g => g.ToDictionary(p => p.NgayLamViec.Day));

            var result = filteredNhanViens.Select(nv =>
            {
                chamCongGroup.TryGetValue(nv.Cccd, out var ccList);
                ccList ??= new List<Domain.Models.ChamCong>();

                decimal empTongGioChuan = 0;
                phanCongGroup.TryGetValue(nv.Cccd, out var empPhanCongs);
                empPhanCongs ??= new Dictionary<int, Domain.Models.PhanCongCa>();

                for (int d = 1; d <= daysInMonth; d++)
                {
                    if (empPhanCongs.TryGetValue(d, out var phanCong))
                    {
                        if (phanCong.IdCaLamViec != null && phanCong.CaLamViec != null)
                        {
                            empTongGioChuan += phanCong.CaLamViec.CalculateWorkingHours();
                        }
                    }
                    else
                    {
                        if (chiTietLichs.TryGetValue(d, out var chiTiet))
                        {
                            if (chiTiet.LoaiNgay == LoaiNgay.NGAY_LAM_VIEC)
                            {
                                empTongGioChuan += chiTiet.CaLamViecMacDinh?.CalculateWorkingHours() ?? 8m;
                            }
                        }
                    }
                }
                
                var empNgayCongChuan = Math.Round(empTongGioChuan / 8m, 3);

                quyetDinhGroup.TryGetValue(nv.Cccd, out var qd);
                string? tenPhongBan = qd?.ChucVuMoi?.PhongBan?.TenPb ?? nv.PhongBan?.TenPb;

                return new ChamCongSummaryDto
                {
                    CccdNhanVien = nv.Cccd,
                    HoTenNhanVien = nv.HoTen,
                    TenPhongBan = tenPhongBan,
                    Thang = request.Thang,
                    Nam = request.Nam,
                    NgayCongChuan = empNgayCongChuan,
                    TongNgayCongThucTe = ccList
                        .Where(cc => cc.LoaiNgayCong == LoaiNgayCong.LAM_DU_CA
                                  || cc.LoaiNgayCong == LoaiNgayCong.NUA_CA
                                  || cc.LoaiNgayCong == LoaiNgayCong.DI_TRE_VE_SOM
                                  || cc.LoaiNgayCong == LoaiNgayCong.VANG_CO_PHEP)
                        .Sum(cc => cc.SoNgayCong),
                    NgayNghiLe = ccList.Count(cc => cc.LoaiNgayCong == LoaiNgayCong.NGHI_LE),
                    NgayNghiCuoiTuan = ccList.Count(cc => cc.LoaiNgayCong == LoaiNgayCong.NGHI_CUOI_TUAN),
                    NgayVangKhongPhep = ccList.Count(cc => cc.LoaiNgayCong == LoaiNgayCong.VANG_KHONG_PHEP),

                };
            }).OrderBy(s => s.TenPhongBan).ThenBy(s => s.HoTenNhanVien).ToList();

            return new Response<List<ChamCongSummaryDto>>(result);
        }
    }
}
