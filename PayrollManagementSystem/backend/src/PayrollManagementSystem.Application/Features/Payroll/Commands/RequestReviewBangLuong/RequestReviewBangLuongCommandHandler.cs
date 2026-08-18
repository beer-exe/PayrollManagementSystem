using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Application.Features.Payroll.Commands.RequestReviewBangLuong
{
    public class RequestReviewBangLuongCommandHandler : IRequestHandler<RequestReviewBangLuongCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public RequestReviewBangLuongCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<Response<bool>> Handle(RequestReviewBangLuongCommand request, CancellationToken cancellationToken)
        {
            var bangLuong = await _context.BangLuongs
                .Include(b => b.KyLuong)
                .Include(b => b.NhanVien)
                .FirstOrDefaultAsync(b => b.IdBangLuong == request.IdBangLuong, cancellationToken);

            if (bangLuong == null)
            {
                throw new ApiException("Không tìm thấy bảng lương.");
            }

            // Kiểm tra quyền (phải là chính nhân viên đó mới được yêu cầu)
            var currentUserId = _currentUserService.UserId;
            if (bangLuong.NhanVien.IdTaiKhoan != currentUserId)
            {
                throw new ApiException("Bạn không có quyền yêu cầu xem xét bảng lương của người khác.");
            }

            // Chỉ được yêu cầu xem xét khi kỳ lương CHƯA chốt
            if (bangLuong.KyLuong.TrangThai == TrangThaiKyLuong.DA_CHOT)
            {
                throw new ApiException("Kỳ lương này đã chốt, không thể gửi yêu cầu xem xét nữa.");
            }

            // Kiểm tra thời hạn 3 ngày kể từ lúc tạo bảng lương
            var createdTimeUtc = bangLuong.CreatedAt.ToUniversalTime();
            if (DateTime.UtcNow > createdTimeUtc.AddDays(3))
            {
                throw new ApiException("Đã quá thời hạn 3 ngày để yêu cầu xem xét bảng lương kể từ ngày tạo.");
            }

            if (bangLuong.TrangThai == TrangThaiBangLuong.YEU_CAU_XEM_XET)
            {
                throw new ApiException("Bạn đã yêu cầu xem xét bảng lương này trước đó.");
            }

            bangLuong.TrangThai = TrangThaiBangLuong.YEU_CAU_XEM_XET;
            bangLuong.LyDoKhieuNai = request.LyDoKhieuNai;

            await _context.SaveChangesAsync(cancellationToken);

            return new Response<bool>(true, "Gửi yêu cầu xem xét bảng lương thành công.");
        }
    }
}
