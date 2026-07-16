using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.DonNghi.DTOs;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Extensions;

namespace PayrollManagementSystem.Application.Features.DonNghi.Queries.GetDonNghiList
{
    public class GetDonNghiListQueryHandler : IRequestHandler<GetDonNghiListQuery, Response<List<DonNghiDto>>>
    {
        private readonly IApplicationDbContext _context;
        public GetDonNghiListQueryHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<List<DonNghiDto>>> Handle(GetDonNghiListQuery request, CancellationToken cancellationToken)
        {
            var query = _context.DonNghis
                .Include(d => d.NhanVien).ThenInclude(nv => nv.PhongBan)
                .Include(d => d.NguoiDuyet)
                .AsQueryable();

            if (request.Thang.HasValue && request.Nam.HasValue)
                query = query.Where(d => d.NgayBatDau.Month == request.Thang || d.NgayKetThuc.Month == request.Thang)
                             .Where(d => d.NgayBatDau.Year == request.Nam || d.NgayKetThuc.Year == request.Nam);
            else if (request.Nam.HasValue)
                query = query.Where(d => d.NgayBatDau.Year == request.Nam || d.NgayKetThuc.Year == request.Nam);

            if (!string.IsNullOrWhiteSpace(request.CccdNhanVien))
                query = query.Where(d => d.CccdNhanVien == request.CccdNhanVien);

            if (!string.IsNullOrWhiteSpace(request.TrangThai) && Enum.TryParse<TrangThaiDonNghi>(request.TrangThai, out var trangThai))
                query = query.Where(d => d.TrangThai == trangThai);

            if (!string.IsNullOrWhiteSpace(request.IdPhongBan))
                query = query.Where(d => d.NhanVien.IdPb == request.IdPhongBan);

            var list = await query
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync(cancellationToken);

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
