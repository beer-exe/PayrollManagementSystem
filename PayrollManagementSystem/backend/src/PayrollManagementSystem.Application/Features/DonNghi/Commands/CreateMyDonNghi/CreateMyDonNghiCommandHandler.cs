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

            // Lookup CCCD from UserId (JWT claim)
            var taiKhoan = await _context.TaiKhoans
                .Include(t => t.NhanVien)
                .FirstOrDefaultAsync(t => t.IdTaiKhoan == request.UserId, cancellationToken);

            if (taiKhoan?.NhanVien == null)
                throw new ApiException("Không tìm thấy thông tin nhân viên liên kết với tài khoản này.");

            var cccd = taiKhoan.NhanVien.Cccd;

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
