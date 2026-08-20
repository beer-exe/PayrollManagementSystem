using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.CompetencyP2.PhieuDanhGia.DTOs;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Extensions;

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

            var today = DateOnly.FromDateTime(DateTime.Today);

            var userQd = await _context.QuyetDinhNhanSus
                .Where(x => x.Cccd == manager.Cccd && x.TrangThai == Domain.Enums.TrangThaiQuyetDinh.HIEU_LUC && x.NgayHieuLuc <= today)
                .OrderByDescending(x => x.NgayHieuLuc)
                .ThenByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            var reportingEmployees = new List<string>();
            if (userQd != null && !string.IsNullOrEmpty(userQd.IdChucVuMoi))
            {
                var reportingPositions = await _context.ChucVus
                    .Where(c => c.IdChucVuQuanLy == userQd.IdChucVuMoi)
                    .Select(c => c.IdChucVu)
                    .ToListAsync(cancellationToken);

                if (reportingPositions.Any())
                {
                    reportingEmployees = await _context.NhanViens
                        .Where(nv => nv.TrangThai == Domain.Enums.TrangThaiNhanVien.DANG_LAM_VIEC)
                        .Select(nv => new
                        {
                            nv.Cccd,
                            LatestQd = nv.QuyetDinhNhanSus
                                .Where(q => q.TrangThai == Domain.Enums.TrangThaiQuyetDinh.HIEU_LUC && q.NgayHieuLuc <= today)
                                .OrderByDescending(q => q.NgayHieuLuc)
                                .ThenByDescending(q => q.CreatedAt)
                                .FirstOrDefault()
                        })
                        .Where(x => x.LatestQd != null && reportingPositions.Contains(x.LatestQd.IdChucVuMoi))
                        .Select(x => x.Cccd)
                        .ToListAsync(cancellationToken);
                }
            }

            var dataRaw = await _context.PhieuDanhGiaNangLucs
                .AsNoTracking()
                .Include(x => x.KyDanhGia)
                .Where(x => (request.IsHr || x.CccdQuanLy == manager.Cccd || reportingEmployees.Contains(x.CccdNhanVien)) && x.TrangThai != Domain.Enums.TrangThaiPhieuDanhGia.CHO_NV_DANH_GIA)
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
                    TrangThai = x.TrangThai,
                    CanEvaluate = (x.CccdQuanLy == manager.Cccd || reportingEmployees.Contains(x.CccdNhanVien))
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
                TrangThai = x.TrangThai.GetDescription(),
                CanEvaluate = x.CanEvaluate
            }).ToList();

            return new Response<IEnumerable<PhieuDanhGiaDto>>(data);
        }
    }
}
