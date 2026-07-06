using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Application.Features.Departments.Commands.ChangePosition
{
    public class ChangePositionCommandHandler : IRequestHandler<ChangePositionCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;
        public ChangePositionCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<bool>> Handle(ChangePositionCommand request, CancellationToken cancellationToken)
        {
            var nhanVien = await _context.NhanViens.FirstOrDefaultAsync(x => x.Cccd == request.Cccd, cancellationToken);
            if (nhanVien == null) throw new Common.Exceptions.ApiException("Nhân viên không tồn tại trong hệ thống.");

            if (await _context.QuyetDinhNhanSus.AnyAsync(x => x.SoQuyetDinh == request.SoQuyetDinh, cancellationToken))
                throw new Common.Exceptions.ApiException($"Số quyết định '{request.SoQuyetDinh}' đã tồn tại.");

            var quyetDinh = new Domain.Models.QuyetDinhNhanSu
            {
                SoQuyetDinh = request.SoQuyetDinh,
                Cccd = request.Cccd,
                LoaiQuyetDinh = "Thay đổi chức vụ",
                IdChucVuMoi = request.IdChucVuMoi,
                IdBacLuongMoi = request.IdBacLuongMoi,
                NgayHieuLuc = DateOnly.FromDateTime(request.NgayHieuLuc),
                TrangThai = TrangThaiQuyetDinh.HIEU_LUC
            };

            _context.QuyetDinhNhanSus.Add(quyetDinh);
            await _context.SaveChangesAsync(cancellationToken);

            return new Response<bool>(true, "Thay đổi chức vụ nhân sự thành công.");
        }
    }
}
