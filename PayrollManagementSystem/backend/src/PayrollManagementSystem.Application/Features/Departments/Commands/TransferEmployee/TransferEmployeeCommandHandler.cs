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

            var phongBanExists = await _context.PhongBans.AnyAsync(pb => pb.IdPb == request.IdPbMoi, cancellationToken);
            if (!phongBanExists)
                throw new ApiException($"Phòng ban mới với mã '{request.IdPbMoi}' không tồn tại.");

            var bacLuongMoi = await _context.BacLuongs.FirstOrDefaultAsync(b => b.IdChucVu == request.IdChucVuMoi, cancellationToken);
            if (bacLuongMoi == null)
                throw new ApiException($"Chưa có cấu hình Bậc lương nào cho chức vụ '{request.IdChucVuMoi}'. Vui lòng thiết lập bậc lương trước khi điều chuyển.");

            nhanVien.IdPb = request.IdPbMoi;

            var quyetDinh = new QuyetDinhNhanSu
            {
                SoQuyetDinh = request.SoQuyetDinh,
                Cccd = request.Cccd,
                LoaiQuyetDinh = "Điều chuyển công tác",
                IdChucVuMoi = request.IdChucVuMoi,
                IdBacLuongMoi = bacLuongMoi.IdBacLuong,
                NgayHieuLuc = request.NgayHieuLuc,
                NguoiKy = request.NguoiKy,
                TrangThai = TrangThaiQuyetDinh.HIEU_LUC
            };

            _context.QuyetDinhNhanSus.Add(quyetDinh);

            await _context.SaveChangesAsync(cancellationToken);

            return new Response<bool>(true, "Điều chuyển công tác thành công.");
        }
    }
}
