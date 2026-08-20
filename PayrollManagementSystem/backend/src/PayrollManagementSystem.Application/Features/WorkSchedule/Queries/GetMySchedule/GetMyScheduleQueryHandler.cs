using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.WorkSchedule.DTOs;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Extensions;

namespace PayrollManagementSystem.Application.Features.WorkSchedule.Queries.GetMySchedule
{
    public class GetMyScheduleQueryHandler : IRequestHandler<GetMyScheduleQuery, Response<IEnumerable<MyScheduleDayDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetMyScheduleQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<IEnumerable<MyScheduleDayDto>>> Handle(GetMyScheduleQuery request, CancellationToken cancellationToken)
        {
            var nhanVien = await _context.NhanViens
                .AsNoTracking()
                .FirstOrDefaultAsync(n => n.IdTaiKhoan == request.UserId, cancellationToken);

            if (nhanVien == null)
            {
                throw new ApiException("Không tìm thấy thông tin nhân viên.");
            }

            var startDate = new DateOnly(request.Nam, request.Thang, 1);
            var endDate = new DateOnly(request.Nam, request.Thang, DateTime.DaysInMonth(request.Nam, request.Thang));

            // 1. Get general schedule (Khung lịch chung)
            var lich = await _context.LichLamViecs
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Nam == request.Nam && l.TrangThai == TrangThaiLichLamViec.HIEU_LUC, cancellationToken);

            List<MyScheduleDayDto> result = new List<MyScheduleDayDto>();

            if (lich != null)
            {
                var chiTiets = await _context.ChiTietLichLamViecs
                    .AsNoTracking()
                    .Include(c => c.CaLamViecMacDinh)
                    .Where(c => c.IdLich == lich.IdLich && c.Ngay.Month == request.Thang)
                    .ToListAsync(cancellationToken);

                foreach (var chiTiet in chiTiets)
                {
                    result.Add(new MyScheduleDayDto
                    {
                        Ngay = chiTiet.Ngay,
                        Thu = chiTiet.Thu,
                        LoaiNgay = chiTiet.LoaiNgay.GetDescription(),
                        TenNgayNghi = chiTiet.TenNgayNghi,
                        IdCaLamViec = chiTiet.IdCaLamViecMacDinh,
                        TenCa = chiTiet.CaLamViecMacDinh?.TenCa,
                        GioBatDau = chiTiet.CaLamViecMacDinh?.GioBatDau,
                        GioKetThuc = chiTiet.CaLamViecMacDinh?.GioKetThuc,
                        XuyenNgay = chiTiet.CaLamViecMacDinh?.XuyenNgay ?? false,
                        LaCaDuocPhanCong = false
                    });
                }
            }
            else
            {
                // Nếu chưa có lịch làm việc chung cho năm, tạo lịch trống
                for (var date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    result.Add(new MyScheduleDayDto
                    {
                        Ngay = date,
                        Thu = GetVietnameseDayOfWeek(date.DayOfWeek),
                        LoaiNgay = "Chưa xếp lịch",
                        LaCaDuocPhanCong = false
                    });
                }
            }

            // 2. Get specific shift assignments (Phân công ca)
            var phanCongCas = await _context.PhanCongCas
                .AsNoTracking()
                .Include(p => p.CaLamViec)
                .Where(p => p.CccdNhanVien == nhanVien.Cccd && p.NgayLamViec >= startDate && p.NgayLamViec <= endDate)
                .ToListAsync(cancellationToken);

            foreach (var pcc in phanCongCas)
            {
                var day = result.FirstOrDefault(d => d.Ngay == pcc.NgayLamViec);
                if (day != null)
                {
                    day.IdCaLamViec = pcc.IdCaLamViec;
                    day.TenCa = pcc.CaLamViec?.TenCa;
                    day.GioBatDau = pcc.CaLamViec?.GioBatDau;
                    day.GioKetThuc = pcc.CaLamViec?.GioKetThuc;
                    day.XuyenNgay = pcc.CaLamViec?.XuyenNgay ?? false;
                    day.LaCaDuocPhanCong = true;
                    if (day.LoaiNgay == "Nghỉ cuối tuần" || day.LoaiNgay == "Nghỉ lễ" || day.LoaiNgay == "Chưa xếp lịch")
                    {
                        day.LoaiNgay = "Ngày làm việc";
                        day.TenNgayNghi = null;
                    }
                }
            }

            // 3. Get approved leave requests (Đơn nghỉ đã duyệt)
            var donNghis = await _context.DonNghis
                .AsNoTracking()
                .Where(d => d.CccdNhanVien == nhanVien.Cccd && d.TrangThai == TrangThaiDonNghi.DA_DUYET &&
                            d.NgayBatDau <= endDate && d.NgayKetThuc >= startDate)
                .ToListAsync(cancellationToken);

            foreach (var donNghi in donNghis)
            {
                foreach (var day in result)
                {
                    if (day.Ngay >= donNghi.NgayBatDau && day.Ngay <= donNghi.NgayKetThuc)
                    {
                        day.CoNghiPhep = true;
                        day.LoaiNghiPhep = donNghi.LoaiNghi.GetDescription();
                    }
                }
            }

            return new Response<IEnumerable<MyScheduleDayDto>>(result.OrderBy(r => r.Ngay), "Lấy lịch làm việc cá nhân thành công.");
        }

        private string GetVietnameseDayOfWeek(DayOfWeek dayOfWeek)
        {
            return dayOfWeek switch
            {
                DayOfWeek.Monday => "Thứ 2",
                DayOfWeek.Tuesday => "Thứ 3",
                DayOfWeek.Wednesday => "Thứ 4",
                DayOfWeek.Thursday => "Thứ 5",
                DayOfWeek.Friday => "Thứ 6",
                DayOfWeek.Saturday => "Thứ 7",
                DayOfWeek.Sunday => "Chủ nhật",
                _ => ""
            };
        }
    }
}
