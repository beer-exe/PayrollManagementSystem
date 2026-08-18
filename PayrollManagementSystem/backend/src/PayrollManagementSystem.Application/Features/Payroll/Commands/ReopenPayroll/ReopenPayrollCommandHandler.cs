using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Application.Features.Payroll.Commands.ReopenPayroll
{
    public class ReopenPayrollCommandHandler : IRequestHandler<ReopenPayrollCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IHrAuthorizationService _hrAuthorizationService;

        public ReopenPayrollCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUserService,
            IHrAuthorizationService hrAuthorizationService)
        {
            _context = context;
            _currentUserService = currentUserService;
            _hrAuthorizationService = hrAuthorizationService;
        }

        public async Task<Response<bool>> Handle(ReopenPayrollCommand request, CancellationToken cancellationToken)
        {
            // Kiểm tra phân quyền: Nghiệp vụ yêu cầu CHỈ HR cấp quản lý mới có quyền mở chốt
            var isHrManager = await _hrAuthorizationService.IsHrManagerAsync(_currentUserService.UserId, cancellationToken);
            if (!isHrManager)
            {
                throw new ApiException("Chỉ HR cấp quản lý mới có quyền mở chốt kỳ lương!");
            }

            var kyLuong = await _context.KyLuongs
                .FirstOrDefaultAsync(x => x.Thang == request.Thang && x.Nam == request.Nam, cancellationToken);

            if (kyLuong == null)
            {
                throw new ApiException($"Kỳ lương tháng {request.Thang}/{request.Nam} chưa được tạo!");
            }

            if (kyLuong.TrangThai != TrangThaiKyLuong.DA_CHOT)
            {
                throw new ApiException($"Kỳ lương tháng {request.Thang}/{request.Nam} hiện chưa chốt, không cần mở lại!");
            }

            kyLuong.TrangThai = TrangThaiKyLuong.CHUA_CHOT;
            kyLuong.LyDoMoChot = request.LyDo;

            await _context.SaveChangesAsync(cancellationToken);

            return new Response<bool>(true, $"Đã mở chốt kỳ lương tháng {request.Thang}/{request.Nam} thành công.");
        }
    }
}
