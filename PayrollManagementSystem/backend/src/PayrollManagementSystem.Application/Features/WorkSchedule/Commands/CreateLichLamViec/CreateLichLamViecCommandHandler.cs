using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.WorkSchedule.Services;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;

namespace PayrollManagementSystem.Application.Features.WorkSchedule.Commands.CreateLichLamViec
{
    public class CreateLichLamViecCommandHandler : IRequestHandler<CreateLichLamViecCommand, Response<Guid>>
    {
        private readonly IApplicationDbContext _context;

        public CreateLichLamViecCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<Guid>> Handle(CreateLichLamViecCommand request, CancellationToken cancellationToken)
        {
            var exists = await _context.LichLamViecs
                .AnyAsync(l => l.Nam == request.Nam, cancellationToken);

            if (exists)
                throw new ApiException($"Lịch làm việc năm {request.Nam} đã tồn tại trong hệ thống.");

            decimal defaultWorkingHours = 8;
            CaLamViec? defaultShift = null;

            if (request.UseDefaultShift && request.DefaultShiftId.HasValue)
            {
                defaultShift = await _context.CaLamViecs
                    .Include(c => c.KhungGioNghis)
                    .FirstOrDefaultAsync(c => c.Id == request.DefaultShiftId.Value, cancellationToken);

                if (defaultShift == null)
                    throw new ApiException("Không tìm thấy ca làm việc mặc định đã chọn.");

                defaultWorkingHours = defaultShift.CalculateWorkingHours();
            }

            var lich = new LichLamViec
            {
                IdLich = Guid.NewGuid(),
                Nam = request.Nam,
                TrangThai = TrangThaiLichLamViec.HIEU_LUC,
                GhiChu = request.GhiChu
            };

            var holidays = VietnamHolidayService.GetHolidays(request.Nam);

            var chiTiets = new List<ChiTietLichLamViec>();
            var startDate = new DateOnly(request.Nam, 1, 1);
            var endDate = new DateOnly(request.Nam, 12, 31);

            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                LoaiNgay loaiNgay;
                string? tenNgayNghi = null;
                decimal soGioLam = defaultWorkingHours;

                if (holidays.TryGetValue(date, out var tenLe))
                {
                    loaiNgay = LoaiNgay.NGHI_LE;
                    tenNgayNghi = tenLe;
                    soGioLam = 0;
                }
                else if (date.DayOfWeek == DayOfWeek.Sunday)
                {
                    loaiNgay = LoaiNgay.NGHI_CUOI_TUAN;
                    tenNgayNghi = "Nghỉ Chủ Nhật";
                    soGioLam = 0;
                }
                else if (date.DayOfWeek == DayOfWeek.Saturday)
                {
                    loaiNgay = LoaiNgay.NGHI_CUOI_TUAN;
                    tenNgayNghi = "Nghỉ Thứ Bảy";
                    soGioLam = 0;
                }
                else
                {
                    loaiNgay = LoaiNgay.NGAY_LAM_VIEC;
                }

                chiTiets.Add(new ChiTietLichLamViec
                {
                    Id = Guid.NewGuid(),
                    IdLich = lich.IdLich,
                    Ngay = date,
                    Thu = VietnamHolidayService.GetDayOfWeekVietnamese(date.DayOfWeek),
                    LoaiNgay = loaiNgay,
                    TenNgayNghi = tenNgayNghi,
                    SoGioLam = soGioLam,
                    IdCaLamViecMacDinh = (request.UseDefaultShift && loaiNgay == LoaiNgay.NGAY_LAM_VIEC) ? request.DefaultShiftId : null,
                    LichLamViec = lich
                });
            }

            await _context.LichLamViecs.AddAsync(lich, cancellationToken);
            await _context.ChiTietLichLamViecs.AddRangeAsync(chiTiets, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new Response<Guid>(lich.IdLich, $"Tạo lịch làm việc năm {request.Nam} thành công. Tổng {chiTiets.Count} ngày.");
        }
    }
}
