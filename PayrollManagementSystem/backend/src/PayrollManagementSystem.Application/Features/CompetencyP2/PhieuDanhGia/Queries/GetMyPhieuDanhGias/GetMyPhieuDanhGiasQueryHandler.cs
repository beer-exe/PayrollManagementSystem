using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.CompetencyP2.PhieuDanhGia.DTOs;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Extensions;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.PhieuDanhGia.Queries.GetMyPhieuDanhGias
{
    public class GetMyPhieuDanhGiasQueryHandler : IRequestHandler<GetMyPhieuDanhGiasQuery, Response<IEnumerable<PhieuDanhGiaDto>>>
    {
        private readonly IApplicationDbContext _context;
        public GetMyPhieuDanhGiasQueryHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<IEnumerable<PhieuDanhGiaDto>>> Handle(GetMyPhieuDanhGiasQuery request, CancellationToken cancellationToken)
        {
            var nhanVien = await _context.NhanViens.FirstOrDefaultAsync(x => x.IdTaiKhoan == request.TaiKhoanId, cancellationToken);
            if (nhanVien == null) return new Response<IEnumerable<PhieuDanhGiaDto>>("Không tìm thấy nhân viên.");

            var dataRaw = await _context.PhieuDanhGiaNangLucs
                .AsNoTracking()
                .Include(x => x.KyDanhGia)
                .Where(x => x.CccdNhanVien == nhanVien.Cccd)
                .Select(x => new
                {
                    IdPhieu = x.IdPhieu,
                    IdKyDanhGia = x.IdKyDanhGia,
                    TenKyDanhGia = x.KyDanhGia.TenKyDanhGia,
                    CccdNhanVien = x.CccdNhanVien,
                    DiemTongHop = x.DiemTongHop,
                    HeSoP2 = x.HeSoP2,
                    XepLoai = x.XepLoai,
                    NhanXetChung = x.NhanXetChung,
                    TrangThai = x.TrangThai
                })
                .ToListAsync(cancellationToken);

            var data = dataRaw.Select(x => new PhieuDanhGiaDto
            {
                IdPhieu = x.IdPhieu,
                IdKyDanhGia = x.IdKyDanhGia,
                TenKyDanhGia = x.TenKyDanhGia,
                CccdNhanVien = x.CccdNhanVien,
                DiemTongHop = x.DiemTongHop,
                HeSoP2 = x.HeSoP2,
                XepLoai = x.XepLoai,
                NhanXetChung = x.NhanXetChung,
                TrangThai = x.TrangThai.GetDescription()
            }).ToList();

            return new Response<IEnumerable<PhieuDanhGiaDto>>(data);
        }
    }
}
