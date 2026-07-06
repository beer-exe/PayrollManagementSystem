using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Application.Features.Positions.Commands.TogglePositionStatus
{
    public class TogglePositionStatusCommandHandler : IRequestHandler<TogglePositionStatusCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;
        public TogglePositionStatusCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<bool>> Handle(TogglePositionStatusCommand request, CancellationToken cancellationToken)
        {
            var chucVu = await _context.ChucVus.FindAsync(new object[] { request.IdChucVu }, cancellationToken);
            if (chucVu == null) throw new Common.Exceptions.ApiException("Chức vụ không tồn tại.");

            if (chucVu.TrangThai == TrangThaiChucVu.HOAT_DONG)
            {
                bool hasActiveEmployee = await _context.QuyetDinhNhanSus.AnyAsync(q => q.IdChucVuMoi == request.IdChucVu && q.TrangThai == TrangThaiQuyetDinh.HIEU_LUC, cancellationToken);
                bool hasActiveBacLuong = await _context.BacLuongs.AnyAsync(b => b.IdChucVu == request.IdChucVu && b.TrangThai == TrangThaiBacLuong.HIEU_LUC, cancellationToken);

                if (hasActiveEmployee || hasActiveBacLuong)
                    throw new Common.Exceptions.ApiException("Lỗi: Không thể vô hiệu hóa chức vụ này vì đang được gắn với nhân sự hoặc bậc lương đang áp dụng.");

                chucVu.TrangThai = TrangThaiChucVu.NGUNG_HOAT_DONG;
            }
            else
            {
                chucVu.TrangThai = TrangThaiChucVu.HOAT_DONG;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return new Response<bool>(true, $"Đã chuyển trạng thái thành: {chucVu.TrangThai}");
        }
    }
}
