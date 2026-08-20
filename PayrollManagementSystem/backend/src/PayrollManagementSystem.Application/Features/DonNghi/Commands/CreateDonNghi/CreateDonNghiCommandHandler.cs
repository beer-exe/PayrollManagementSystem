using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Application.Features.DonNghi.Commands.CreateDonNghi
{
    public class CreateDonNghiCommandHandler : IRequestHandler<CreateDonNghiCommand, Response<Guid>>
    {
        private readonly IApplicationDbContext _context;
        public CreateDonNghiCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<Guid>> Handle(CreateDonNghiCommand request, CancellationToken cancellationToken)
        {
            if (!Enum.TryParse<LoaiNghi>(request.LoaiNghi, out var loaiNghi))
                throw new ApiException($"Loại nghỉ không hợp lệ: {request.LoaiNghi}");

            var nhanVien = await _context.NhanViens
                .FirstOrDefaultAsync(nv => nv.Cccd == request.CccdNhanVien && !nv.IsDeleted, cancellationToken);
            if (nhanVien == null)
                throw new ApiException("Nhân viên không tồn tại.");

            if (request.SoNgayNghi <= 0)
                throw new ApiException("Số ngày nghỉ phải lớn hơn 0.");

            if (request.NgayBatDau.Year != request.NgayKetThuc.Year)
                throw new ApiException("Ngày bắt đầu và ngày kết thúc của đơn nghỉ phải cùng nằm trong một năm.");

            var hasLich = await _context.LichLamViecs.AnyAsync(l => l.Nam == request.NgayBatDau.Year, cancellationToken);
            if (!hasLich)
                throw new ApiException($"Chưa có lịch làm việc nào được tạo cho năm {request.NgayBatDau.Year}. Vui lòng liên hệ HR để tạo lịch làm việc trước khi nộp đơn.");

            var chiTietLich = await _context.ChiTietLichLamViecs
                .Where(c => c.Ngay >= request.NgayBatDau && c.Ngay <= request.NgayKetThuc)
                .ToListAsync(cancellationToken);

            decimal maxSoNgay = 0;
            if (loaiNghi == LoaiNghi.NGHI_THAI_SAN)
            {
                maxSoNgay = request.NgayKetThuc.DayNumber - request.NgayBatDau.DayNumber + 1;
            }
            else
            {
                maxSoNgay = chiTietLich.Count(c => c.LoaiNgay == LoaiNgay.NGAY_LAM_VIEC);

                // Fallback in case HR hasn't created the schedule yet
                if (chiTietLich.Count == 0)
                {
                    maxSoNgay = request.NgayKetThuc.DayNumber - request.NgayBatDau.DayNumber + 1;
                }
            }

            if (request.SoNgayNghi > maxSoNgay && maxSoNgay > 0)
                throw new ApiException($"Số ngày nghỉ yêu cầu ({request.SoNgayNghi}) vượt quá số ngày được phép nghỉ trong khoảng thời gian này ({maxSoNgay} ngày).");


            var donNghi = new Domain.Models.DonNghi
            {
                CccdNhanVien = request.CccdNhanVien,
                LoaiNghi = loaiNghi,
                NgayBatDau = request.NgayBatDau,
                NgayKetThuc = request.NgayKetThuc,
                SoNgayNghi = request.SoNgayNghi,
                LyDo = request.LyDo,
                TaiLieuDinhKem = request.TaiLieuDinhKem,
                TrangThai = TrangThaiDonNghi.CHO_DUYET,
            };

            await _context.DonNghis.AddAsync(donNghi, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new Response<Guid>(donNghi.Id, "Tạo đơn nghỉ thành công.");
        }
    }
}
