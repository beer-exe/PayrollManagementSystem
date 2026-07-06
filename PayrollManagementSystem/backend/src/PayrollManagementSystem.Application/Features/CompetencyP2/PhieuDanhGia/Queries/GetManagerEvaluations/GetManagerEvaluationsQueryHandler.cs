using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Application.Features.CompetencyP2.PhieuDanhGia.DTOs;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.PhieuDanhGia.Queries.GetManagerEvaluations
{
    public class GetManagerEvaluationsQueryHandler : IRequestHandler<GetManagerEvaluationsQuery, Response<IEnumerable<PhieuDanhGiaDto>>>
    {
        private readonly IApplicationDbContext _context;
        public GetManagerEvaluationsQueryHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<IEnumerable<PhieuDanhGiaDto>>> Handle(GetManagerEvaluationsQuery request, CancellationToken cancellationToken)
        {
            var manager = await _context.NhanViens.FirstOrDefaultAsync(x => x.IdTaiKhoan == request.TaiKhoanId, cancellationToken);
            if (manager == null) return new Response<IEnumerable<PhieuDanhGiaDto>>("Không tìm thấy tài khoản quản lý.");

            var data = await _context.PhieuDanhGiaNangLucs
                .Include(x => x.KyDanhGia)
                .Where(x => (request.IsHr || x.CccdQuanLy == manager.Cccd) && x.TrangThai != Domain.Enums.TrangThaiPhieuDanhGia.CHO_NV_DANH_GIA)
                .Select(x => new PhieuDanhGiaDto
                {
                    IdPhieu = x.IdPhieu,
                    IdKyDanhGia = x.IdKyDanhGia,
                    TenKyDanhGia = x.KyDanhGia.TenKyDanhGia,
                    CccdNhanVien = x.CccdNhanVien,
                    DiemTongHop = x.DiemTongHop,
                    HeSoP2 = x.HeSoP2,
                    XepLoai = x.XepLoai,
                    NhanXetChung = x.NhanXetChung,
                    TrangThai = x.TrangThai.ToString(),
                    CanEvaluate = (x.CccdQuanLy == manager.Cccd)
                })
                .ToListAsync(cancellationToken);

            return new Response<IEnumerable<PhieuDanhGiaDto>>(data);
        }
    }
}
