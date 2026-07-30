using MediatR;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;

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
            // 1. Kiểm tra kỳ lương trước đã chốt chưa (nếu có)
            var prevMonth = request.Thang == 1 ? 12 : request.Thang - 1;
            var prevYear = request.Thang == 1 ? request.Nam - 1 : request.Nam;
            
            var prevKyLuong = await _context.KyLuongs
                .FirstOrDefaultAsync(x => x.Thang == prevMonth && x.Nam == prevYear, cancellationToken);
                
            if (prevKyLuong != null && prevKyLuong.TrangThai == TrangThaiKyLuong.CHUA_CHOT)
            {
                throw new ApiException($"Không thể chạy lương tháng {request.Thang}/{request.Nam} vì kỳ lương tháng {prevMonth}/{prevYear} chưa được chốt!");
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

            // Xóa dữ liệu bảng lương cũ của kỳ này nếu có
            var oldBangLuongs = await _context.BangLuongs.Where(x => x.IdKyLuong == kyLuong.IdKyLuong).ToListAsync(cancellationToken);
            if (oldBangLuongs.Any())
            {
                _context.BangLuongs.RemoveRange(oldBangLuongs);
                await _context.SaveChangesAsync(cancellationToken);
            }

            // 3. Lọc danh sách nhân viên đủ điều kiện (Có quyết định nhân sự và đang làm việc)
            var activeEmployees = await _context.NhanViens
                .Where(x => x.TrangThai == Domain.Enums.TrangThaiNhanVien.DANG_LAM_VIEC)
                .ToListAsync(cancellationToken);

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

            var listBangLuong = new List<BangLuong>();

            foreach (var nv in activeEmployees)
            {
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
                    }
                }
                decimal ngayCongChuan = Math.Round(empTongGioChuan / 8m, 3);
                if (ngayCongChuan == 0) ngayCongChuan = 21.375m; // Fallback nếu dữ liệu lỗi
                
                decimal gioCongChuan = empTongGioChuan;
                decimal gioCongThucTe = empTongGioThucTe;
                if (gioCongChuan == 0) gioCongChuan = ngayCongChuan * 8m;

                // Nếu không có giờ công thực tế -> Bỏ qua không tính lương
                if (gioCongThucTe <= 0) continue;

                // Lấy Quyết định nhân sự có hiệu lực cuối cùng trong tháng
                var qd = await _context.QuyetDinhNhanSus
                    .Include(x => x.BacLuong)
                    .Where(x => x.Cccd == nv.Cccd 
                             && x.TrangThai != TrangThaiQuyetDinh.HUY_BO 
                             && x.NgayHieuLuc <= endOfMonth
                             && (x.NgayHetHan == null || x.NgayHetHan >= startOfMonth))
                    .OrderByDescending(x => x.NgayHieuLuc)
                    .FirstOrDefaultAsync(cancellationToken);

                if (qd == null || qd.BacLuong == null) continue;

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

                // Công thức chuẩn theo ý (Cách 1: Lương 3P = P1 * P2 * P3)
                decimal luong3P = p1 * heSoP2 * heSoP3;

                // Lương thời gian = (Lương 3P * Giờ công thực tế) / Giờ công chuẩn
                decimal luongThoiGian = (luong3P * gioCongThucTe) / gioCongChuan;
                
                // Do P3 đã tính thẳng vào Lương 3P và Lương thời gian nên cục Lương hiệu suất để riêng = 0
                decimal luongHieuSuat = 0;

                decimal tongThuNhap = luongThoiGian + luongHieuSuat;
                
                // Thuế, bảo hiểm = 0
                decimal thucLinh = tongThuNhap;

                var bangLuong = new BangLuong
                {
                    IdKyLuong = kyLuong.IdKyLuong,
                    CccdNhanVien = nv.Cccd,
                    Thang = request.Thang,
                    Nam = request.Nam,
                    P1 = p1,
                    HeSoP2 = heSoP2,
                    HeSoP3 = heSoP3,
                    NgayCongChuan = Math.Round(ngayCongChuan, 3),
                    NgayCongThucTe = Math.Round(ngayCongThucTe, 3),
                    GioCongChuan = Math.Round(gioCongChuan, 2),
                    GioCongThucTe = Math.Round(gioCongThucTe, 2),
                    LuongThoiGian = Math.Round(luongThoiGian, 0),
                    LuongHieuSuatP3 = Math.Round(luongHieuSuat, 0),
                    PhuCap = 0,
                    Thuong = 0,
                    TangCa = 0,
                    Phat = 0,
                    TruBaoHiem = 0,
                    TruThue = 0,
                    TongThuNhap = Math.Round(tongThuNhap, 0),
                    ThucLinh = Math.Round(thucLinh, 0),
                };

                listBangLuong.Add(bangLuong);
            }

            _context.BangLuongs.AddRange(listBangLuong);
            await _context.SaveChangesAsync(cancellationToken);

            return new Response<bool>(true, "Tính lương thành công");
        }
    }
}
