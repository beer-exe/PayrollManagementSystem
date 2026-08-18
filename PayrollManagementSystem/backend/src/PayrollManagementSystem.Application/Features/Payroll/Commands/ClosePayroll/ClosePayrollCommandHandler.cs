using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Application.Features.Payroll.Commands.ClosePayroll
{
    public class ClosePayrollCommandHandler : IRequestHandler<ClosePayrollCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IHrAuthorizationService _hrAuthorizationService;

        public ClosePayrollCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUserService,
            IHrAuthorizationService hrAuthorizationService)
        {
            _context = context;
            _currentUserService = currentUserService;
            _hrAuthorizationService = hrAuthorizationService;
        }

        public async Task<Response<bool>> Handle(ClosePayrollCommand request, CancellationToken cancellationToken)
        {
            var kyLuong = await _context.KyLuongs
                .FirstOrDefaultAsync(x => x.Thang == request.Thang && x.Nam == request.Nam, cancellationToken);

            if (kyLuong == null)
            {
                throw new ApiException($"Kỳ lương tháng {request.Thang}/{request.Nam} chưa được tạo!");
            }

            if (kyLuong.TrangThai != TrangThaiKyLuong.CHUA_CHOT)
            {
                throw new ApiException($"Kỳ lương tháng {request.Thang}/{request.Nam} đã được chốt hoặc thanh toán!");
            }

            // Kiểm tra xem đã có bảng lương nào chưa
            var hasBangLuong = await _context.BangLuongs.AnyAsync(x => x.IdKyLuong == kyLuong.IdKyLuong, cancellationToken);
            if (!hasBangLuong)
            {
                throw new ApiException($"Không thể chốt kỳ lương tháng {request.Thang}/{request.Nam} vì chưa có dữ liệu bảng lương (hãy ấn Tính lương trước).");
            }

            // Bắt buộc tất cả nhân viên phải xác nhận bảng lương thì mới được chốt kỳ lương, ngoại trừ đã quá hạn 3 ngày
            var unconfirmedBangLuongs = await _context.BangLuongs
                .Where(x => x.IdKyLuong == kyLuong.IdKyLuong && x.TrangThai != TrangThaiBangLuong.DA_XAC_NHAN)
                .ToListAsync(cancellationToken);
            
            if (unconfirmedBangLuongs.Any())
            {
                // Kiểm tra xem có bảng lương nào đang YEU_CAU_XEM_XET không
                var inReviewCount = unconfirmedBangLuongs.Count(x => x.TrangThai == TrangThaiBangLuong.YEU_CAU_XEM_XET);
                if (inReviewCount > 0)
                {
                    throw new ApiException($"Không thể chốt kỳ lương vì có {inReviewCount} phiếu lương đang có Yêu cầu xem xét cần HR giải quyết!");
                }

                var now = DateTimeOffset.UtcNow;
                var notExpiredCount = unconfirmedBangLuongs.Count(x => (now - (x.UpdatedAt ?? x.CreatedAt)).TotalDays <= 3);
                
                if (notExpiredCount > 0)
                {
                    throw new ApiException($"Không thể chốt kỳ lương vì có {notExpiredCount} phiếu lương chưa được nhân viên xác nhận và chưa quá hạn 3 ngày chờ!");
                }

                // Tự động xác nhận các phiếu lương đã quá 3 ngày
                foreach (var bl in unconfirmedBangLuongs)
                {
                    bl.TrangThai = TrangThaiBangLuong.DA_XAC_NHAN;
                    bl.GhiChu = string.IsNullOrEmpty(bl.GhiChu) 
                        ? "Tự động xác nhận do quá hạn 3 ngày" 
                        : bl.GhiChu + " (Tự động xác nhận do quá hạn 3 ngày)";
                }
                
                _context.BangLuongs.UpdateRange(unconfirmedBangLuongs);
            }

            // Kiểm tra ràng buộc thời gian: Nếu chốt trước ngày kết thúc kỳ lương, chỉ HR cấp quản lý mới có quyền
            var today = DateOnly.FromDateTime(DateTime.Today);
            if (today < kyLuong.NgayKetThuc)
            {
                var isHrManager = await _hrAuthorizationService.IsHrManagerAsync(_currentUserService.UserId, cancellationToken);
                if (!isHrManager)
                {
                    throw new ApiException($"Không thể chốt kỳ lương tháng {request.Thang}/{request.Nam} trước khi kết thúc tháng (ngày {kyLuong.NgayKetThuc:dd/MM/yyyy}). Chỉ HR cấp quản lý mới có quyền chốt trước thời hạn!");
                }
            }

            kyLuong.TrangThai = TrangThaiKyLuong.DA_CHOT;

            await _context.SaveChangesAsync(cancellationToken);

            return new Response<bool>(true, $"Đã chốt kỳ lương tháng {request.Thang}/{request.Nam} thành công.");
        }
    }
}
