using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Application.Features.CompetencyP2.PhieuDanhGia.DTOs;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.PhieuDanhGia.Queries.GetPhieuDanhGiaById
{
    public class GetPhieuDanhGiaByIdQueryHandler : IRequestHandler<GetPhieuDanhGiaByIdQuery, Response<PhieuDanhGiaDto>>
    {
        private readonly IApplicationDbContext _context;
        public GetPhieuDanhGiaByIdQueryHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<PhieuDanhGiaDto>> Handle(GetPhieuDanhGiaByIdQuery request, CancellationToken cancellationToken)
        {
            var p = await _context.PhieuDanhGiaNangLucs
                .Include(x => x.KyDanhGia)
                .Include(x => x.ChiTietDanhGias)
                .ThenInclude(c => c.TieuChi)
                .FirstOrDefaultAsync(x => x.IdPhieu == request.IdPhieu, cancellationToken);

            if (p == null) return new Response<PhieuDanhGiaDto>("Không tìm thấy phiếu đánh giá.");

            var user = await _context.NhanViens.FirstOrDefaultAsync(x => x.IdTaiKhoan == request.TaiKhoanId, cancellationToken);
            if (user == null) return new Response<PhieuDanhGiaDto>("Không tìm thấy tài khoản hợp lệ.");

            if (!request.IsHr && p.CccdNhanVien != user.Cccd && p.CccdQuanLy != user.Cccd)
            {
                return new Response<PhieuDanhGiaDto>("Bạn không có quyền xem phiếu đánh giá này.");
            }

            var dto = new PhieuDanhGiaDto
            {
                IdPhieu = p.IdPhieu,
                IdKyDanhGia = p.IdKyDanhGia,
                TenKyDanhGia = p.KyDanhGia.TenKyDanhGia,
                CccdNhanVien = p.CccdNhanVien,
                DiemTongHop = p.DiemTongHop,
                HeSoP2 = p.HeSoP2,
                XepLoai = p.XepLoai,
                NhanXetChung = p.NhanXetChung,
                TrangThai = p.TrangThai.ToString(),
                CanEvaluate = (p.CccdQuanLy == user.Cccd),
                ChiTietDanhGias = p.ChiTietDanhGias.Select(c => new ChiTietDanhGiaDto
                {
                    IdChiTiet = c.IdChiTiet,
                    IdTieuChi = c.IdTieuChi,
                    TenNangLuc = c.TieuChi.TenNangLuc,
                    YeuCauToiThieu = c.TieuChi.YeuCauToiThieu,
                    TyTrong = c.TieuChi.TyTrong,
                    DiemTuDanhGia = c.DiemTuDanhGia,
                    DiemQuanLyDanhGia = c.DiemQuanLyDanhGia,
                    NhanXetNhanVien = c.NhanXetNhanVien,
                    NhanXetQuanLy = c.NhanXetQuanLy
                }).ToList()
            };

            return new Response<PhieuDanhGiaDto>(dto);
        }
    }
}
