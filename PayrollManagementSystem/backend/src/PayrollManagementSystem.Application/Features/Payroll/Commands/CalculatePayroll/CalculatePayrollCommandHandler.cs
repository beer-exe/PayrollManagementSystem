using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;

namespace PayrollManagementSystem.Application.Features.Payroll.Commands.CalculatePayroll
{
    public class CalculatePayrollCommandHandler : IRequestHandler<CalculatePayrollCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;

        public CalculatePayrollCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<bool>> Handle(CalculatePayrollCommand request, CancellationToken cancellationToken)
        {
            // 1. Kiểm tra xem có kỳ lương nào trước đó chưa chốt không
            var unclosedKyLuong = await _context.KyLuongs
                .Where(x => (x.Nam < request.Nam) || (x.Nam == request.Nam && x.Thang < request.Thang))
                .Where(x => x.TrangThai == TrangThaiKyLuong.CHUA_CHOT)
                .OrderBy(x => x.Nam).ThenBy(x => x.Thang)
                .FirstOrDefaultAsync(cancellationToken);

            if (unclosedKyLuong != null)
            {
                throw new ApiException($"Không thể chạy lương tháng {request.Thang}/{request.Nam} vì kỳ lương tháng {unclosedKyLuong.Thang}/{unclosedKyLuong.Nam} chưa được chốt!");
            }

            // Kiểm tra xem Kỳ chấm công của tháng này đã tồn tại và đã chốt chưa
            var kyChamCong = await _context.KyChamCongs
                .FirstOrDefaultAsync(x => x.Thang == request.Thang && x.Nam == request.Nam, cancellationToken);
            if (kyChamCong == null)
            {
                throw new ApiException($"Không thể tính lương vì kỳ chấm công tháng {request.Thang}/{request.Nam} không tồn tại!");
            }
            else if (kyChamCong.TrangThai != TrangThaiKyChamCong.DA_CHOT)
            {
                throw new ApiException($"Không thể tính lương vì kỳ chấm công tháng {request.Thang}/{request.Nam} chưa được chốt!");
            }

            // 2. Tạo hoặc lấy Kỳ lương hiện tại
            var kyLuong = await _context.KyLuongs
                .FirstOrDefaultAsync(x => x.Thang == request.Thang && x.Nam == request.Nam, cancellationToken);

            if (kyLuong == null)
            {
                kyLuong = new KyLuong
                {
                    Thang = request.Thang,
                    Nam = request.Nam,
                    TenKyLuong = $"Bảng lương tháng {request.Thang}/{request.Nam}",
                    NgayBatDau = new DateOnly(request.Nam, request.Thang, 1),
                    NgayKetThuc = new DateOnly(request.Nam, request.Thang, DateTime.DaysInMonth(request.Nam, request.Thang)),
                    TrangThai = TrangThaiKyLuong.CHUA_CHOT
                };
                _context.KyLuongs.Add(kyLuong);
                await _context.SaveChangesAsync(cancellationToken);
            }
            else if (kyLuong.TrangThai != TrangThaiKyLuong.CHUA_CHOT)
            {
                throw new ApiException("Kỳ lương này đã chốt hoặc đã thanh toán, không thể tính lại!");
            }

            // Lấy dữ liệu bảng lương cũ (CHUA_XAC_NHAN) để update thay vì xóa, giữ nguyên Id
            var oldBangLuongs = await _context.BangLuongs
                .Where(x => x.IdKyLuong == kyLuong.IdKyLuong && x.TrangThai == TrangThaiBangLuong.CHUA_XAC_NHAN)
                .ToListAsync(cancellationToken);

            // 3. Lọc danh sách nhân viên đủ điều kiện (Có quyết định nhân sự và đang làm việc)
            var activeEmployees = await _context.NhanViens
                .Where(x => x.TrangThai == Domain.Enums.TrangThaiNhanVien.DANG_LAM_VIEC)
                .ToListAsync(cancellationToken);
            if (!activeEmployees.Any())
            {
                throw new ApiException("Không thể tính lương vì không có nhân viên nào đang làm việc trong kỳ này.");
            }

            // 4. Lấy dữ liệu chấm công của tháng (để tính Số ngày công thực tế)
            var chamCongs = await _context.ChamCongs
                .Where(x => x.NgayChamCong.Month == request.Thang && x.NgayChamCong.Year == request.Nam && x.TrangThai == TrangThaiChamCong.DA_XAC_NHAN)
                .ToListAsync(cancellationToken);

            var chiTietLichs = await _context.ChiTietLichLamViecs
                .Include(ct => ct.CaLamViecMacDinh)
                    .ThenInclude(c => c.KhungGioNghis)
                .Where(ct => ct.Ngay.Month == request.Thang && ct.Ngay.Year == request.Nam)
                .ToDictionaryAsync(ct => ct.Ngay.Day, cancellationToken);

            var phanCongCas = await _context.PhanCongCas
                .Include(p => p.CaLamViec)
                    .ThenInclude(c => c.KhungGioNghis)
                .Where(p => p.NgayLamViec.Month == request.Thang && p.NgayLamViec.Year == request.Nam)
                .ToListAsync(cancellationToken);

            var phanCongGroup = phanCongCas
                .GroupBy(p => p.CccdNhanVien)
                .ToDictionary(g => g.Key, g => g.ToDictionary(p => p.NgayLamViec.Day));

            var daysInMonth = DateTime.DaysInMonth(request.Nam, request.Thang);
            var startOfMonth = new DateOnly(request.Nam, request.Thang, 1);
            var endOfMonth = new DateOnly(request.Nam, request.Thang, daysInMonth);

            // Lấy danh sách Phiếu đánh giá năng lực (P2)
            var phieuDanhGias = await _context.PhieuDanhGiaNangLucs
                .Include(p => p.KyDanhGia)
                .Where(p => p.TrangThai == TrangThaiPhieuDanhGia.DA_HOAN_THANH
                         && p.KyDanhGia.NgayBatDau <= endOfMonth
                         && p.KyDanhGia.NgayKetThuc >= startOfMonth)
                .ToListAsync(cancellationToken);

            var phieuP2Group = phieuDanhGias
                .GroupBy(p => p.CccdNhanVien)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderByDescending(p => p.KyDanhGia.NgayKetThuc).FirstOrDefault()
                );

            // Lấy danh sách Phiếu KPI (P3)
            var phieuKpis = await _context.PhieuKpis
                .Include(p => p.KyKpi)
                .Where(p => p.TrangThai == TrangThaiPhieuKpi.DA_PHE_DUYET
                         && p.KyKpi.Thang == request.Thang
                         && p.KyKpi.Nam == request.Nam)
                .ToListAsync(cancellationToken);

            var phieuKpiGroup = phieuKpis
                .GroupBy(p => p.CccdNhanVien)
                .ToDictionary(
                    g => g.Key,
                    g => g.FirstOrDefault()
                );

            // Lấy danh sách khoản khấu trừ đang kích hoạt
            var activeKhoanKhauTrus = await _context.KhoanKhauTrus.Where(x => x.IsActive).ToListAsync(cancellationToken);

            // Lấy dữ liệu Thuế TNCN
            var cauHinhGiamTru = await _context.CauHinhGiamTrus.FirstOrDefaultAsync(cancellationToken);
            var giamTruBanThan = cauHinhGiamTru?.GiamTruBanThan ?? 11000000m;
            var giamTruNguoiPhuThuoc = cauHinhGiamTru?.GiamTruNguoiPhuThuoc ?? 4400000m;
            var bacThues = await _context.BacThues.OrderBy(b => b.Bac).ToListAsync(cancellationToken);

            // Lấy số lượng người phụ thuộc (Đếm các ThanNhanNhanVien có cờ LaNguoiPhuThuoc = true)
            var nptGroups = await _context.TNhanNviens
                .Where(t => t.LaNguoiPhuThuoc)
                .GroupBy(t => t.Cccd)
                .Select(g => new { Cccd = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.Cccd, g => g.Count, cancellationToken);

            var listBangLuong = new List<BangLuong>();

            // Lấy danh sách ID nhân viên đã có bảng lương đang ở trạng thái khác CHUA_XAC_NHAN (tức là đã xác nhận hoặc đang khiếu nại)
            var existingConfirmedCccds = await _context.BangLuongs
                .Where(x => x.IdKyLuong == kyLuong.IdKyLuong && x.TrangThai != TrangThaiBangLuong.CHUA_XAC_NHAN)
                .Select(x => x.CccdNhanVien)
                .ToListAsync(cancellationToken);

            foreach (var nv in activeEmployees)
            {
                if (existingConfirmedCccds.Contains(nv.Cccd)) continue;

                // Phân tách chi tiết các loại ngày công để tính lương
                var nvChamCong = chamCongs.Where(x => x.CccdNhanVien == nv.Cccd).ToList();

                // 1. Công làm việc thực tế và Nghỉ phép có lương (Vắng có phép được duyệt sẽ có SoNgayCong > 0)
                decimal ngayDiLam = nvChamCong
                    .Where(cc => cc.LoaiNgayCong == LoaiNgayCong.LAM_DU_CA ||
                                 cc.LoaiNgayCong == LoaiNgayCong.NUA_CA ||
                                 cc.LoaiNgayCong == LoaiNgayCong.DI_TRE_VE_SOM ||
                                 cc.LoaiNgayCong == LoaiNgayCong.VANG_CO_PHEP)
                    .Sum(x => x.SoNgayCong);

                // 2. Nghỉ lễ (Hưởng nguyên lương, đếm số ngày)
                int soNgayNghiLe = nvChamCong.Count(cc => cc.LoaiNgayCong == LoaiNgayCong.NGHI_LE);

                // 3. Vắng không phép (Không hưởng lương, không cộng vào)
                // int soNgayVangKhongPhep = nvChamCong.Count(cc => cc.LoaiNgayCong == LoaiNgayCong.VANG_KHONG_PHEP);

                // Tổng ngày công để tính lương = Ngày đi làm + Nghỉ phép có lương + Nghỉ lễ
                decimal ngayCongThucTe = ngayDiLam + soNgayNghiLe;

                // Tính Giờ công chuẩn & Giờ công thực tế
                decimal empTongGioChuan = 0;
                decimal empTongGioThucTe = 0;
                phanCongGroup.TryGetValue(nv.Cccd, out var empPhanCongs);
                empPhanCongs ??= new Dictionary<int, Domain.Models.PhanCongCa>();

                for (int d = 1; d <= daysInMonth; d++)
                {
                    decimal hoursForDay = 0;
                    if (empPhanCongs.TryGetValue(d, out var phanCong))
                    {
                        if (phanCong.IdCaLamViec != null && phanCong.CaLamViec != null)
                        {
                            hoursForDay = phanCong.CaLamViec.CalculateWorkingHours();
                        }
                    }
                    else
                    {
                        if (chiTietLichs.TryGetValue(d, out var chiTiet))
                        {
                            if (chiTiet.LoaiNgay == LoaiNgay.NGAY_LAM_VIEC)
                            {
                                hoursForDay = chiTiet.CaLamViecMacDinh?.CalculateWorkingHours() ?? 8m;
                            }
                        }
                    }
                    empTongGioChuan += hoursForDay;

                    var chamCongDay = nvChamCong.FirstOrDefault(x => x.NgayChamCong.Day == d);
                    if (chamCongDay != null)
                    {
                        if (chamCongDay.LoaiNgayCong == LoaiNgayCong.LAM_DU_CA ||
                            chamCongDay.LoaiNgayCong == LoaiNgayCong.NUA_CA ||
                            chamCongDay.LoaiNgayCong == LoaiNgayCong.DI_TRE_VE_SOM)
                        {
                            empTongGioThucTe += chamCongDay.SoGioLamThucTe;
                        }
                        else if (chamCongDay.LoaiNgayCong == LoaiNgayCong.VANG_CO_PHEP ||
                                 chamCongDay.LoaiNgayCong == LoaiNgayCong.NGHI_LE)
                        {
                            empTongGioThucTe += hoursForDay;
                        }
                        // VANG_CO_PHEP_KHONG_LUONG does not add to empTongGioThucTe
                    }
                }
                decimal ngayCongChuan = Math.Round(empTongGioChuan / 8m, 3);
                if (ngayCongChuan == 0) ngayCongChuan = 21.375m; // Fallback nếu dữ liệu lỗi

                decimal gioCongChuan = empTongGioChuan;
                decimal gioCongThucTe = empTongGioThucTe;
                if (gioCongChuan == 0) gioCongChuan = ngayCongChuan * 8m;

                // Tìm bảng lương cũ nếu có
                var oldRecord = oldBangLuongs.FirstOrDefault(x => x.CccdNhanVien == nv.Cccd);

                // Nếu không có giờ công thực tế -> Bỏ qua không tính lương
                if (gioCongThucTe <= 0)
                {
                    if (oldRecord != null) oldRecord.IsDeleted = true;
                    continue;
                }

                // Lấy Quyết định nhân sự có hiệu lực cuối cùng trong tháng
                var qd = await _context.QuyetDinhNhanSus
                    .Include(x => x.BacLuong)
                    .Where(x => x.Cccd == nv.Cccd
                             && x.TrangThai != TrangThaiQuyetDinh.HUY_BO
                             && x.NgayHieuLuc <= endOfMonth
                             && (x.NgayHetHan == null || x.NgayHetHan >= startOfMonth))
                    .OrderByDescending(x => x.NgayHieuLuc)
                    .FirstOrDefaultAsync(cancellationToken);

                if (qd == null || qd.BacLuong == null)
                {
                    if (oldRecord != null) oldRecord.IsDeleted = true;
                    continue;
                }

                // P1
                decimal p1 = qd.BacLuong.LuongP1;

                // P2
                decimal heSoP2 = 1.0m;
                if (phieuP2Group.TryGetValue(nv.Cccd, out var phieuP2) && phieuP2?.HeSoP2 != null)
                {
                    heSoP2 = phieuP2.HeSoP2.Value;
                }

                // P3
                decimal heSoP3 = 1.0m; // Mặc định
                if (phieuKpiGroup.TryGetValue(nv.Cccd, out var phieuP3) && phieuP3 != null)
                {
                    heSoP3 = phieuP3.HeSoP3;
                }

                // Công thức chuẩn theo ý (Cách 1: Lương 3P = P1 * P2 * P3)
                decimal luong3P = p1 * heSoP2 * heSoP3;

                // Lương thời gian = (Lương 3P * Giờ công thực tế) / Giờ công chuẩn
                decimal luongThoiGian = (luong3P * gioCongThucTe) / gioCongChuan;

                // Do P3 đã tính thẳng vào Lương 3P và Lương thời gian nên cục Lương hiệu suất để riêng = 0
                decimal luongHieuSuat = 0;

                decimal tongThuNhap = luongThoiGian + luongHieuSuat;

                // Khấu trừ
                decimal tongKhauTru = 0;
                var listChiTietKhauTru = new List<object>();
                foreach (var khauTru in activeKhoanKhauTrus)
                {
                    decimal soTienTru = 0;
                    if (khauTru.LoaiCongThuc == LoaiCongThucKhauTru.TY_LE_PHAN_TRAM)
                    {
                        soTienTru = (khauTru.GiaTri / 100m) * p1;
                    }
                    else if (khauTru.LoaiCongThuc == LoaiCongThucKhauTru.SO_TIEN_CO_DINH)
                    {
                        soTienTru = khauTru.GiaTri;
                    }
                    tongKhauTru += soTienTru;
                    listChiTietKhauTru.Add(new
                    {
                        ten = khauTru.TenKhoanKhauTru,
                        soTien = Math.Round(soTienTru, 0)
                    });
                }

                decimal thucLinh = tongThuNhap - tongKhauTru;
                string chiTietKhauTruJson = System.Text.Json.JsonSerializer.Serialize(listChiTietKhauTru);

                // --- TÍNH THUẾ TNCN LŨY TIẾN ---
                decimal thuNhapTruocThue = tongThuNhap - tongKhauTru; // Không trừ phạt

                // Số người phụ thuộc
                int soNguoiPhuThuoc = nptGroups.TryGetValue(nv.Cccd, out int nptCount) ? nptCount : 0;
                decimal tongGiamTru = giamTruBanThan + (soNguoiPhuThuoc * giamTruNguoiPhuThuoc);

                decimal thuNhapTinhThue = Math.Max(0, thuNhapTruocThue - tongGiamTru);
                decimal truThue = 0;

                var listChiTietBacThue = new List<object>();

                if (thuNhapTinhThue > 0)
                {
                    foreach (var bac in bacThues)
                    {
                        if (thuNhapTinhThue > bac.TuGia)
                        {
                            decimal maxInBracket = bac.DenGia ?? decimal.MaxValue;
                            decimal taxableInBracket = Math.Min(thuNhapTinhThue, maxInBracket) - bac.TuGia;
                            decimal taxForBracket = taxableInBracket * (bac.ThueSuat / 100m);
                            truThue += taxForBracket;

                            listChiTietBacThue.Add(new
                            {
                                bac = bac.Bac,
                                thueSuat = bac.ThueSuat,
                                thuNhapTinh = Math.Round(taxableInBracket, 0),
                                soTien = Math.Round(taxForBracket, 0)
                            });
                        }
                    }
                }

                var thueDetails = new
                {
                    thuNhapTruocThue = Math.Round(thuNhapTruocThue, 0),
                    soNguoiPhuThuoc = soNguoiPhuThuoc,
                    tongGiamTru = Math.Round(tongGiamTru, 0),
                    thuNhapTinhThue = Math.Round(thuNhapTinhThue, 0),
                    chiTietBacThue = listChiTietBacThue
                };
                string chiTietThueJson = System.Text.Json.JsonSerializer.Serialize(thueDetails);

                // Tiền phạt hiện tại mặc định là 0 (sẽ lấy từ dữ liệu nếu có module Phạt sau này)
                decimal phat = 0;

                // Tính lại Thực Lĩnh sau khi trừ thuế và trừ phạt
                thucLinh = tongThuNhap - tongKhauTru - truThue - phat;

                // Tìm bảng lương cũ nếu có, nếu không thì tạo mới
                var bangLuong = oldBangLuongs.FirstOrDefault(x => x.CccdNhanVien == nv.Cccd);
                bool isNew = false;
                if (bangLuong == null)
                {
                    bangLuong = new BangLuong();
                    isNew = true;
                }

                bangLuong.IdKyLuong = kyLuong.IdKyLuong;
                bangLuong.CccdNhanVien = nv.Cccd;
                bangLuong.Thang = request.Thang;
                bangLuong.Nam = request.Nam;
                bangLuong.P1 = p1;
                bangLuong.HeSoP2 = heSoP2;
                bangLuong.HeSoP3 = heSoP3;
                bangLuong.NgayCongChuan = Math.Round(ngayCongChuan, 3);
                bangLuong.NgayCongThucTe = Math.Round(ngayCongThucTe, 3);
                bangLuong.GioCongChuan = Math.Round(gioCongChuan, 2);
                bangLuong.GioCongThucTe = Math.Round(gioCongThucTe, 2);
                bangLuong.LuongThoiGian = Math.Round(luongThoiGian, 0);
                bangLuong.LuongHieuSuatP3 = Math.Round(luongHieuSuat, 0);
                bangLuong.PhuCap = 0;
                bangLuong.Thuong = 0;
                bangLuong.TangCa = 0;
                bangLuong.Phat = phat;
                bangLuong.KhauTru = Math.Round(tongKhauTru, 0);
                bangLuong.ChiTietKhauTru = chiTietKhauTruJson;
                bangLuong.TruThue = Math.Round(truThue, 0);
                bangLuong.ChiTietThue = chiTietThueJson;
                bangLuong.TongThuNhap = Math.Round(tongThuNhap, 0);
                bangLuong.ThucLinh = Math.Round(thucLinh, 0);

                if (isNew)
                {
                    listBangLuong.Add(bangLuong);
                }
                else
                {
                    _context.BangLuongs.Update(bangLuong);
                }
            }

            _context.BangLuongs.AddRange(listBangLuong);
            await _context.SaveChangesAsync(cancellationToken);

            return new Response<bool>(true, "Tính lương thành công");
        }
    }
}
