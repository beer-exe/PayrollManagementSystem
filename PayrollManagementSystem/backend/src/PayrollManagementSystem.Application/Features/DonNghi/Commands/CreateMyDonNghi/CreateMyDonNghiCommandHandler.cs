using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Application.Features.DonNghi.Commands.CreateMyDonNghi
{
    public class CreateMyDonNghiCommandHandler : IRequestHandler<CreateMyDonNghiCommand, Response<Guid>>
    {
        private readonly IApplicationDbContext _context;

        public CreateMyDonNghiCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<Guid>> Handle(CreateMyDonNghiCommand request, CancellationToken cancellationToken)
        {
            if (!Enum.TryParse<LoaiNghi>(request.LoaiNghi, out var loaiNghi))
                throw new ApiException($"Loại nghỉ không hợp lệ: {request.LoaiNghi}");

            var taiKhoan = await _context.TaiKhoans
                .Include(t => t.NhanVien)
                .FirstOrDefaultAsync(t => t.IdTaiKhoan == request.UserId, cancellationToken);

            if (taiKhoan?.NhanVien == null)
                throw new ApiException("Không tìm thấy thông tin nhân viên liên kết với tài khoản này.");

            var cccd = taiKhoan.NhanVien.Cccd;

            if (request.SoNgayNghi <= 0)
                throw new ApiException("Số ngày nghỉ phải lớn hơn 0.");

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
                
                if (chiTietLich.Count == 0)
                {
                    maxSoNgay = request.NgayKetThuc.DayNumber - request.NgayBatDau.DayNumber + 1;
                    // throw new ApiException("Lịch làm việc chưa được thiết lập cho khoảng thời gian này.");
                }
            }

            if (request.SoNgayNghi > maxSoNgay && maxSoNgay > 0)
                throw new ApiException($"Số ngày nghỉ yêu cầu ({request.SoNgayNghi}) vượt quá số ngày được phép nghỉ trong khoảng thời gian này ({maxSoNgay} ngày).");


            var donNghi = new Domain.Models.DonNghi
            {
                CccdNhanVien = cccd,
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

            return new Response<Guid>(donNghi.Id, "Nộp đơn xin nghỉ thành công.");
        }
    }
}
