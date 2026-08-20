using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Infrastructure.Services
{
    public class TimekeepingCalculatorService : ITimekeepingCalculatorService
    {
        private readonly IApplicationDbContext _context;
        private const decimal GRACE_PERIOD_PHUT = 15m;

        public TimekeepingCalculatorService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TimekeepingResult> CalculateTimekeepingAsync(
            string cccdNhanVien,
            DateOnly ngayChamCong,
            TimeOnly? gioVaoThucTe,
            TimeOnly? gioRaThucTe,
            CancellationToken cancellationToken = default)
        {
            var result = new TimekeepingResult();

            // Lấy ca làm việc dự kiến từ PhanCongCa hoặc ChiTietLichLamViec
            var phanCongCa = await _context.PhanCongCas
                .Include(p => p.CaLamViec)
                .FirstOrDefaultAsync(p => p.CccdNhanVien == cccdNhanVien && p.NgayLamViec == ngayChamCong, cancellationToken);

            Guid? idCaLamViecToUse = null;

            if (phanCongCa != null)
            {
                if (phanCongCa.IdCaLamViec == null)
                {
                    // Được phân nghỉ (OFF_DAY) chủ động
                    return new TimekeepingResult
                    {
                        SoGioLamThucTe = 0,
                        SoNgayCong = 0,
                        LoaiNgayCong = LoaiNgayCong.NGHI_CUOI_TUAN,
                        GhiChu = "Được gán ngày nghỉ trong phân công ca"
                    };
                }
                idCaLamViecToUse = phanCongCa.IdCaLamViec;
            }
            else
            {
                // Fallback lịch làm việc mặc định
                var chiTietLich = await _context.ChiTietLichLamViecs
                    .FirstOrDefaultAsync(ct => ct.Ngay == ngayChamCong, cancellationToken);

                if (chiTietLich?.LoaiNgay == LoaiNgay.NGHI_LE)
                {
                    return new TimekeepingResult { SoGioLamThucTe = 0, SoNgayCong = 0, LoaiNgayCong = LoaiNgayCong.NGHI_LE };
                }
                if (chiTietLich?.LoaiNgay == LoaiNgay.NGHI_CUOI_TUAN && chiTietLich?.IdCaLamViecMacDinh == null)
                {
                    return new TimekeepingResult { SoGioLamThucTe = 0, SoNgayCong = 0, LoaiNgayCong = LoaiNgayCong.NGHI_CUOI_TUAN };
                }

                idCaLamViecToUse = chiTietLich?.IdCaLamViecMacDinh;
            }

            if (idCaLamViecToUse == null)
            {
                // Không có ca làm việc nào được cấu hình
                return new TimekeepingResult
                {
                    SoGioLamThucTe = 0,
                    SoNgayCong = 0,
                    LoaiNgayCong = LoaiNgayCong.NGHI_CUOI_TUAN,
                    GhiChu = "Không tìm thấy ca làm việc tiêu chuẩn"
                };
            }

            var caLamViec = await _context.CaLamViecs
                .Include(c => c.KhungGioNghis)
                .FirstOrDefaultAsync(c => c.Id == idCaLamViecToUse, cancellationToken);

            if (caLamViec == null)
            {
                return new TimekeepingResult { SoGioLamThucTe = 0, SoNgayCong = 0, LoaiNgayCong = LoaiNgayCong.VANG_KHONG_PHEP, GhiChu = "Ca làm việc bị xóa" };
            }

            if (gioVaoThucTe == null || gioRaThucTe == null)
            {
                return new TimekeepingResult
                {
                    SoGioLamThucTe = 0,
                    SoNgayCong = 0,
                    LoaiNgayCong = LoaiNgayCong.VANG_KHONG_PHEP,
                    GhiChu = "Thiếu giờ vào hoặc giờ ra"
                };
            }

            var isNightShift = caLamViec.XuyenNgay || caLamViec.GioKetThuc < caLamViec.GioBatDau;

            // Xử lý intersection
            var shiftStart = caLamViec.GioBatDau;
            var shiftEnd = caLamViec.GioKetThuc;
            if (isNightShift && shiftEnd < shiftStart) shiftEnd = shiftEnd.Add(TimeSpan.FromDays(1));

            var actualStart = gioVaoThucTe.Value.ToTimeSpan();
            // Cân nhắc giờ ra nếu ca qua đêm
            var actualEnd = gioRaThucTe.Value.ToTimeSpan();
            if (isNightShift && actualEnd < TimeSpan.FromHours(12)) // giả định giờ ra vào buổi sáng hôm sau
            {
                actualEnd = actualEnd.Add(TimeSpan.FromDays(1));
            }

            // Xử lý giờ ra bé hơn giờ vào (điều chỉnh thủ công bị lỗi, hoặc night shift k khớp)
            if (actualEnd < actualStart)
            {
                actualEnd = actualEnd.Add(TimeSpan.FromDays(1));
            }

            // Tính phút đi trễ, về sớm
            int lateMinutes = 0;
            if (actualStart > shiftStart)
            {
                lateMinutes = (int)(actualStart - shiftStart).TotalMinutes;
            }

            int earlyMinutes = 0;
            if (actualEnd < shiftEnd)
            {
                earlyMinutes = (int)(shiftEnd - actualEnd).TotalMinutes;
            }

            // Intersection (Thời gian hữu ích làm việc trong ca)
            var effectiveStart = actualStart > shiftStart ? actualStart : shiftStart;
            var effectiveEnd = actualEnd < shiftEnd ? actualEnd : shiftEnd;

            if (effectiveEnd <= effectiveStart)
            {
                return new TimekeepingResult
                {
                    SoGioLamThucTe = 0,
                    SoNgayCong = 0,
                    LoaiNgayCong = LoaiNgayCong.VANG_KHONG_PHEP,
                    SoPhutDiTre = lateMinutes,
                    SoPhutVeSom = earlyMinutes,
                    GhiChu = "Chấm công nằm ngoài giờ hành chính của ca"
                };
            }

            var totalEffectiveDuration = (effectiveEnd - effectiveStart).TotalHours;

            // Trừ giờ nghỉ (nếu có giờ nghỉ giao với giờ hữu ích)
            if (caLamViec.KhungGioNghis != null)
            {
                foreach (var breakTime in caLamViec.KhungGioNghis.Where(b => !b.TinhVaoGioLam && !b.IsDeleted))
                {
                    var breakStart = breakTime.GioBatDau;
                    var breakEnd = breakTime.GioKetThuc;

                    if (breakEnd < breakStart) breakEnd = breakEnd.Add(TimeSpan.FromDays(1));
                    if (isNightShift && breakStart < TimeSpan.FromHours(12)) breakStart = breakStart.Add(TimeSpan.FromDays(1));
                    if (isNightShift && breakEnd < TimeSpan.FromHours(12)) breakEnd = breakEnd.Add(TimeSpan.FromDays(1));

                    var breakEffectiveStart = effectiveStart > breakStart ? effectiveStart : breakStart;
                    var breakEffectiveEnd = effectiveEnd < breakEnd ? effectiveEnd : breakEnd;

                    if (breakEffectiveEnd > breakEffectiveStart)
                    {
                        totalEffectiveDuration -= (breakEffectiveEnd - breakEffectiveStart).TotalHours;
                    }
                }
            }

            var soGioHieuQua = (decimal)Math.Round(Math.Max(totalEffectiveDuration, 0), 2);
            var soGioChuan = caLamViec.CalculateWorkingHours();
            if (soGioChuan <= 0) soGioChuan = 8m; // Fallback an toàn

            var gracePeriodGio = GRACE_PERIOD_PHUT / 60m;
            var shiftCompletionRatio = soGioHieuQua / soGioChuan; // Tỷ lệ hoàn thành ca để xác định trạng thái
            var soNgayCong = Math.Round(soGioHieuQua / 8m, 3); // Quy đổi công chuẩn tuyệt đối hệ cơ số 8 giờ
            LoaiNgayCong loaiNgayCong;

            if (shiftCompletionRatio >= 1m - (gracePeriodGio / soGioChuan))
            {
                loaiNgayCong = LoaiNgayCong.LAM_DU_CA;
            }
            else if (shiftCompletionRatio >= 0.5m - (gracePeriodGio / soGioChuan) && shiftCompletionRatio < 1m)
            {
                loaiNgayCong = Math.Abs(shiftCompletionRatio - 0.5m) < 0.05m
                    ? LoaiNgayCong.NUA_CA
                    : LoaiNgayCong.DI_TRE_VE_SOM;
            }
            else
            {
                loaiNgayCong = soNgayCong == 0 ? LoaiNgayCong.VANG_KHONG_PHEP : LoaiNgayCong.DI_TRE_VE_SOM;
            }

            result.SoGioLamThucTe = soGioHieuQua;
            result.SoNgayCong = soNgayCong;
            result.LoaiNgayCong = loaiNgayCong;
            result.SoPhutDiTre = lateMinutes;
            result.SoPhutVeSom = earlyMinutes;

            return result;
        }
    }
}
