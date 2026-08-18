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

            // Bắt buộc tất cả nhân viên phải xác nhận bảng lương thì mới được chốt kỳ lương
            var unconfirmedBangLuongsCount = await _context.BangLuongs
                .CountAsync(x => x.IdKyLuong == kyLuong.IdKyLuong && x.TrangThai != TrangThaiBangLuong.DA_XAC_NHAN, cancellationToken);
            
            if (unconfirmedBangLuongsCount > 0)
            {
                throw new ApiException($"Không thể chốt kỳ lương tháng {request.Thang}/{request.Nam} vì vẫn còn {unconfirmedBangLuongsCount} nhân viên chưa xác nhận bảng lương!");
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
