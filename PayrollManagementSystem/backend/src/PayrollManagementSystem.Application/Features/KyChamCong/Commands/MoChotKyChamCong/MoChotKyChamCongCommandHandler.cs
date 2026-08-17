using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Application.Features.KyChamCong.Commands.MoChotKyChamCong
{
    public class MoChotKyChamCongCommandHandler : IRequestHandler<MoChotKyChamCongCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IHrAuthorizationService _authService;
        private readonly ICurrentUserService _currentUserService;

        public MoChotKyChamCongCommandHandler(IApplicationDbContext context, IHrAuthorizationService authService, ICurrentUserService currentUserService)
        {
            _context = context;
            _authService = authService;
            _currentUserService = currentUserService;
        }

        public async Task<Response<bool>> Handle(MoChotKyChamCongCommand request, CancellationToken cancellationToken)
        {
            // Kiểm tra quyền của HR (chỉ HR cấp quản lý mới được mở chốt)
            var userId = _currentUserService.UserId;
            var isManager = await _authService.IsHrManagerAsync(userId, cancellationToken);
            if (!isManager)
            {
                throw new ApiException("Chỉ Quản lý Hành chính - Nhân sự mới có quyền mở chốt kỳ chấm công.");
            }

            var kyChamCong = await _context.KyChamCongs
                .FirstOrDefaultAsync(x => x.Thang == request.Thang && x.Nam == request.Nam, cancellationToken);

            if (kyChamCong == null)
            {
                throw new ApiException($"Không tìm thấy kỳ chấm công tháng {request.Thang}/{request.Nam}.");
            }

            if (kyChamCong.TrangThai == TrangThaiKyChamCong.DANG_MO)
            {
                throw new ApiException($"Kỳ chấm công tháng {request.Thang}/{request.Nam} đang mở.");
            }

            // Mở chốt
            kyChamCong.TrangThai = TrangThaiKyChamCong.DANG_MO;
            _context.KyChamCongs.Update(kyChamCong);
            
            // Unlink all ChamCong records for this month
            var chamCongs = await _context.ChamCongs
                .Where(x => x.IdKyChamCong == kyChamCong.Id)
                .ToListAsync(cancellationToken);

            foreach (var cc in chamCongs)
            {
                cc.IdKyChamCong = null;
                cc.TrangThai = TrangThaiChamCong.CHUA_XAC_NHAN;
            }

            if (chamCongs.Any())
            {
                _context.ChamCongs.UpdateRange(chamCongs);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return new Response<bool>(true, $"Đã mở chốt kỳ chấm công tháng {request.Thang}/{request.Nam} thành công.");
        }
    }
}
