using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.DonNghi.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.DonNghi.Queries.GetMyNgayPhep
{
    public class GetMyNgayPhepQueryHandler : IRequestHandler<GetMyNgayPhepQuery, Response<NgayPhepDto?>>
    {
        private readonly IApplicationDbContext _context;

        public GetMyNgayPhepQueryHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<NgayPhepDto?>> Handle(GetMyNgayPhepQuery request, CancellationToken cancellationToken)
        {
            var taiKhoan = await _context.TaiKhoans
                .Include(t => t.NhanVien).ThenInclude(nv => nv!.PhongBan)
                .FirstOrDefaultAsync(t => t.IdTaiKhoan == request.UserId, cancellationToken);

            if (taiKhoan?.NhanVien == null)
                throw new ApiException("Không tìm thấy thông tin nhân viên liên kết với tài khoản này.");

            var cccd = taiKhoan.NhanVien.Cccd;

            var ngayPhep = await _context.NgayPhepNhanViens
                .FirstOrDefaultAsync(n => n.CccdNhanVien == cccd && n.Nam == request.Nam, cancellationToken);

            if (ngayPhep == null)
                return new Response<NgayPhepDto?>(null);

            var dto = new NgayPhepDto
            {
                Id = ngayPhep.Id,
                CccdNhanVien = ngayPhep.CccdNhanVien,
                HoTenNhanVien = taiKhoan.NhanVien.HoTen,
                TenPhongBan = taiKhoan.NhanVien.PhongBan?.TenPb,
                Nam = ngayPhep.Nam,
                TongNgayPhep = ngayPhep.TongNgayPhep,
                DaSuDung = ngayPhep.DaSuDung,
                ConLai = ngayPhep.ConLai,
            };

            return new Response<NgayPhepDto?>(dto);
        }
    }
}
