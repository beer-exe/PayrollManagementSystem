using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.PhieuDanhGia.Commands.GenerateMyPhieuDanhGia
{
    public class GenerateMyPhieuDanhGiaCommandHandler : IRequestHandler<GenerateMyPhieuDanhGiaCommand, Response<Guid>>
    {
        private readonly IApplicationDbContext _context;
        public GenerateMyPhieuDanhGiaCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<Guid>> Handle(GenerateMyPhieuDanhGiaCommand request, CancellationToken cancellationToken)
        {
            var nhanVien = await _context.NhanViens.FirstOrDefaultAsync(x => x.IdTaiKhoan == request.TaiKhoanId, cancellationToken);
            if (nhanVien == null) return new Response<Guid>("Không tìm thấy nhân viên hợp lệ.");

            var cccd = nhanVien.Cccd;

            var existing = await _context.PhieuDanhGiaNangLucs
                .FirstOrDefaultAsync(x => x.IdKyDanhGia == request.IdKyDanhGia && x.CccdNhanVien == cccd, cancellationToken);
            if (existing != null)
                return new Response<Guid>(existing.IdPhieu, "Phiếu đã tồn tại.");

            var quyetDinh = await _context.QuyetDinhNhanSus
                .Where(x => x.Cccd == cccd && x.TrangThai == Domain.Enums.TrangThaiQuyetDinh.HIEU_LUC)
                .OrderByDescending(x => x.NgayHieuLuc)
                .FirstOrDefaultAsync(cancellationToken);

            if (quyetDinh == null || string.IsNullOrEmpty(quyetDinh.IdChucVuMoi))
                return new Response<Guid>("Nhân viên chưa có chức vụ hoặc quyết định nhân sự hợp lệ.");

            var khungNangLucs = await _context.KhungNangLucP2s
                .Where(x => x.IdChucVu == quyetDinh.IdChucVuMoi)
                .ToListAsync(cancellationToken);

            if (!khungNangLucs.Any())
                return new Response<Guid>("Chức vụ này chưa được thiết lập Khung Năng Lực P2.");

            // Determine manager based on ChucVuQuanLy
            string? managerCccd = null;
            var chucVuHienTai = await _context.ChucVus.FirstOrDefaultAsync(c => c.IdChucVu == quyetDinh.IdChucVuMoi, cancellationToken);
            if (chucVuHienTai != null && !string.IsNullOrEmpty(chucVuHienTai.IdChucVuQuanLy))
            {
                var managerQd = await _context.QuyetDinhNhanSus
                    .Where(q => q.IdChucVuMoi == chucVuHienTai.IdChucVuQuanLy && q.TrangThai == Domain.Enums.TrangThaiQuyetDinh.HIEU_LUC)
                    .OrderByDescending(q => q.NgayHieuLuc)
                    .FirstOrDefaultAsync(cancellationToken);
                
                if (managerQd != null)
                {
                    managerCccd = managerQd.Cccd;
                }
            }

            var phieu = new Domain.Models.PhieuDanhGiaNangLuc
            {
                IdKyDanhGia = request.IdKyDanhGia,
                CccdNhanVien = cccd,
                CccdQuanLy = managerCccd,
                TrangThai = Domain.Enums.TrangThaiPhieuDanhGia.CHO_NV_DANH_GIA
            };

            foreach (var tc in khungNangLucs)
            {
                phieu.ChiTietDanhGias.Add(new Domain.Models.ChiTietDanhGiaNangLuc
                {
                    IdTieuChi = tc.IdTieuChi
                });
            }

            _context.PhieuDanhGiaNangLucs.Add(phieu);
            await _context.SaveChangesAsync(cancellationToken);

            return new Response<Guid>(phieu.IdPhieu, "Đã tạo phiếu đánh giá thành công.");
        }
    }
}
