using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Application.Features.Payroll.Commands.ConfirmBangLuong
{
    public class ConfirmBangLuongCommandHandler : IRequestHandler<ConfirmBangLuongCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public ConfirmBangLuongCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<Response<bool>> Handle(ConfirmBangLuongCommand request, CancellationToken cancellationToken)
        {
            var bangLuong = await _context.BangLuongs
                .Include(b => b.KyLuong)
                .Include(b => b.NhanVien)
                .FirstOrDefaultAsync(b => b.IdBangLuong == request.IdBangLuong, cancellationToken);

            if (bangLuong == null)
            {
                throw new ApiException("Không tìm thấy bảng lương.");
            }

            // Kiểm tra quyền (phải là chính nhân viên đó mới được xác nhận)
            var currentUserId = _currentUserService.UserId;
            if (bangLuong.NhanVien.IdTaiKhoan != currentUserId)
            {
                throw new ApiException("Bạn không có quyền xác nhận bảng lương của người khác.");
            }

            // Chỉ được xác nhận khi kỳ lương CHƯA chốt (nhân viên phải xác nhận hết thì mới được chốt)
            if (bangLuong.KyLuong.TrangThai == TrangThaiKyLuong.DA_CHOT)
            {
                throw new ApiException("Kỳ lương này đã chốt, không thể thay đổi trạng thái xác nhận nữa.");
            }

            // Kiểm tra thời hạn 3 ngày kể từ lúc tạo bảng lương
            var createdTimeUtc = bangLuong.CreatedAt.ToUniversalTime();
            if (DateTime.UtcNow > createdTimeUtc.AddDays(3))
            {
                throw new ApiException("Đã quá thời hạn 3 ngày để xác nhận bảng lương kể từ ngày tạo.");
            }

            if (bangLuong.TrangThai == TrangThaiBangLuong.DA_XAC_NHAN)
            {
                throw new ApiException("Bảng lương đã được xác nhận trước đó.");
            }

            bangLuong.TrangThai = TrangThaiBangLuong.DA_XAC_NHAN;
            bangLuong.LyDoKhieuNai = null; // Xóa lý do khiếu nại cũ nếu có

            await _context.SaveChangesAsync(cancellationToken);

            return new Response<bool>(true, "Xác nhận bảng lương thành công.");
        }
    }
}
