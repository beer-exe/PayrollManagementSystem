using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;

namespace PayrollManagementSystem.Application.Features.Departments.Commands.TransferEmployee
{
    public class TransferEmployeeCommandHandler : IRequestHandler<TransferEmployeeCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;
        public TransferEmployeeCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<bool>> Handle(TransferEmployeeCommand request, CancellationToken cancellationToken)
        {
            if (await _context.QuyetDinhNhanSus.AnyAsync(q => q.SoQuyetDinh == request.SoQuyetDinh, cancellationToken))
                throw new ApiException($"Số quyết định '{request.SoQuyetDinh}' đã tồn tại.");

            var nhanVien = await _context.NhanViens.FirstOrDefaultAsync(nv => nv.Cccd == request.Cccd, cancellationToken);
            if (nhanVien == null)
                throw new ApiException($"Không tìm thấy nhân viên với CCCD '{request.Cccd}'.");

            var quyetDinhHienTai = await _context.QuyetDinhNhanSus
                .Where(qd => qd.Cccd == request.Cccd && qd.TrangThai == TrangThaiQuyetDinh.HIEU_LUC)
                .OrderByDescending(qd => qd.NgayHieuLuc)
                .ThenByDescending(qd => qd.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            var phongBanExists = await _context.PhongBans.AnyAsync(pb => pb.IdPb == request.IdPbMoi, cancellationToken);
            if (!phongBanExists)
                throw new ApiException($"Phòng ban mới với mã '{request.IdPbMoi}' không tồn tại.");

            var chucVuMoi = await _context.ChucVus.FindAsync(new object[] { request.IdChucVuMoi }, cancellationToken);
            if (chucVuMoi?.IdNgachLuong == null)
                throw new ApiException($"Chức vụ mới không được gán Ngạch lương hợp lệ.");

            var bacLuongMoi = await _context.BacLuongs.FirstOrDefaultAsync(b => b.IdBacLuong == request.IdBacLuongMoi && b.IdNgachLuong == chucVuMoi.IdNgachLuong, cancellationToken);
            if (bacLuongMoi == null)
                throw new ApiException($"Bậc lương '{request.IdBacLuongMoi}' không hợp lệ hoặc không thuộc ngạch lương của chức vụ mới.");

            nhanVien.IdPb = request.IdPbMoi;

            var quyetDinh = new QuyetDinhNhanSu
            {
                SoQuyetDinh = request.SoQuyetDinh,
                Cccd = request.Cccd,
                LoaiQuyetDinh = string.IsNullOrWhiteSpace(request.LoaiQuyetDinh) ? "Điều chuyển công tác" : request.LoaiQuyetDinh,
                IdChucVuCu = quyetDinhHienTai?.IdChucVuMoi,
                IdBacLuongCu = quyetDinhHienTai?.IdBacLuongMoi,
                IdChucVuMoi = request.IdChucVuMoi,
                IdBacLuongMoi = bacLuongMoi.IdBacLuong,
                NgayHieuLuc = request.NgayHieuLuc,
                NguoiKy = request.NguoiKy,
                TrangThai = TrangThaiQuyetDinh.HIEU_LUC
            };

            _context.QuyetDinhNhanSus.Add(quyetDinh);

            if (quyetDinhHienTai != null && request.NgayHieuLuc <= DateOnly.FromDateTime(DateTime.Today))
            {
                quyetDinhHienTai.TrangThai = TrangThaiQuyetDinh.HET_HAN;
                quyetDinhHienTai.NgayHetHan = request.NgayHieuLuc;
                _context.QuyetDinhNhanSus.Update(quyetDinhHienTai);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return new Response<bool>(true, "Điều chuyển công tác thành công.");
        }
    }
}
