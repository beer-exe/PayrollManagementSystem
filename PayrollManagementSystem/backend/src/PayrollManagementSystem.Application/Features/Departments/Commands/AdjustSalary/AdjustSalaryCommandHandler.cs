using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;

namespace PayrollManagementSystem.Application.Features.Departments.Commands.AdjustSalary
{
    public class AdjustSalaryCommandHandler : IRequestHandler<AdjustSalaryCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;
        public AdjustSalaryCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<bool>> Handle(AdjustSalaryCommand request, CancellationToken cancellationToken)
        {
            var nhanVien = await _context.NhanViens.FirstOrDefaultAsync(x => x.Cccd == request.Cccd, cancellationToken);
            if (nhanVien == null) throw new Common.Exceptions.ApiException("Nhân viên không tồn tại trong hệ thống.");

            if (await _context.QuyetDinhNhanSus.AnyAsync(x => x.SoQuyetDinh == request.SoQuyetDinh, cancellationToken))
                throw new Common.Exceptions.ApiException($"Số quyết định '{request.SoQuyetDinh}' đã tồn tại. Vui lòng nhập số khác.");

            var quyetDinhHienTai = await _context.QuyetDinhNhanSus
                .Where(qd => qd.Cccd == request.Cccd && qd.TrangThai == TrangThaiQuyetDinh.HIEU_LUC)
                .OrderByDescending(qd => qd.NgayHieuLuc)
                .FirstOrDefaultAsync(cancellationToken);

            var quyetDinh = new Domain.Models.QuyetDinhNhanSu
            {
                SoQuyetDinh = request.SoQuyetDinh,
                Cccd = request.Cccd,
                LoaiQuyetDinh = "Điều chỉnh lương",
                IdChucVuMoi = quyetDinhHienTai?.IdChucVuMoi, // Giữ nguyên chức vụ cũ
                IdBacLuongMoi = request.IdBacLuongMoi,
                NgayHieuLuc = DateOnly.FromDateTime(request.NgayHieuLuc),
                TrangThai = TrangThaiQuyetDinh.HIEU_LUC
            };

            _context.QuyetDinhNhanSus.Add(quyetDinh);
            await _context.SaveChangesAsync(cancellationToken);

            return new Response<bool>(true, "Điều chỉnh bậc lương thành công.");
        }
    }
}
