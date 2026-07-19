using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.DonNghi.DTOs;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Extensions;

namespace PayrollManagementSystem.Application.Features.DonNghi.Queries.GetMyDonNghiList
{
    public class GetMyDonNghiListQueryHandler : IRequestHandler<GetMyDonNghiListQuery, Response<List<DonNghiDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetMyDonNghiListQueryHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<List<DonNghiDto>>> Handle(GetMyDonNghiListQuery request, CancellationToken cancellationToken)
        {
            var taiKhoan = await _context.TaiKhoans
                .Include(t => t.NhanVien)
                .FirstOrDefaultAsync(t => t.IdTaiKhoan == request.UserId, cancellationToken);

            if (taiKhoan?.NhanVien == null)
                throw new ApiException("Không tìm thấy thông tin nhân viên liên kết với tài khoản này.");

            var cccd = taiKhoan.NhanVien.Cccd;

            var query = _context.DonNghis
                .Include(d => d.NhanVien).ThenInclude(nv => nv.PhongBan)
                .Include(d => d.NguoiDuyet)
                .Where(d => d.CccdNhanVien == cccd)
                .AsQueryable();

            if (request.Thang.HasValue && request.Nam.HasValue)
                query = query.Where(d => (d.NgayBatDau.Month == request.Thang || d.NgayKetThuc.Month == request.Thang)
                                      && (d.NgayBatDau.Year == request.Nam || d.NgayKetThuc.Year == request.Nam));
            else if (request.Nam.HasValue)
                query = query.Where(d => d.NgayBatDau.Year == request.Nam || d.NgayKetThuc.Year == request.Nam);

            if (!string.IsNullOrWhiteSpace(request.TrangThai) && Enum.TryParse<TrangThaiDonNghi>(request.TrangThai, out var trangThai))
                query = query.Where(d => d.TrangThai == trangThai);

            if (!string.IsNullOrWhiteSpace(request.LoaiNghi) && Enum.TryParse<LoaiNghi>(request.LoaiNghi, out var loaiNghi))
                query = query.Where(d => d.LoaiNghi == loaiNghi);

            var list = await query.OrderByDescending(d => d.CreatedAt).ToListAsync(cancellationToken);

            var result = list.Select(d => new DonNghiDto
            {
                Id = d.Id,
                CccdNhanVien = d.CccdNhanVien,
                HoTenNhanVien = d.NhanVien.HoTen,
                TenPhongBan = d.NhanVien.PhongBan?.TenPb,
                LoaiNghi = d.LoaiNghi.GetDescription(),
                NgayBatDau = d.NgayBatDau.ToString("yyyy-MM-dd"),
                NgayKetThuc = d.NgayKetThuc.ToString("yyyy-MM-dd"),
                SoNgayNghi = d.SoNgayNghi,
                LyDo = d.LyDo,
                TaiLieuDinhKem = d.TaiLieuDinhKem,
                TrangThai = d.TrangThai.GetDescription(),
                HoTenNguoiDuyet = d.NguoiDuyet?.HoTen,
                LyDoTuChoi = d.LyDoTuChoi,
                NgayDuyet = d.NgayDuyet,
                NgayTao = d.CreatedAt.DateTime,
            }).ToList();

            return new Response<List<DonNghiDto>>(result);
        }
    }
}
