using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Users.Commands.ChangePassword
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ICurrentUserService _currentUserService;

        public ChangePasswordCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher, ICurrentUserService currentUserService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _currentUserService = currentUserService;
        }

        public async Task<Response<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (userId == null)
            {
                throw new ApiException("Không tìm thấy thông tin phiên đăng nhập.");
            }

            var taiKhoan = await _context.TaiKhoans
                .FirstOrDefaultAsync(t => t.IdTaiKhoan == userId, cancellationToken);

            if (taiKhoan == null)
            {
                throw new ApiException("Tài khoản không tồn tại.");
            }

            if (!_passwordHasher.VerifyPasswordEnhanced(request.OldPassword, taiKhoan.MatKhauHash))
            {
                throw new ApiException("Mật khẩu cũ không chính xác.");
            }

            taiKhoan.MatKhauHash = _passwordHasher.HashPasswordEnhanced(request.NewPassword);

            await _context.SaveChangesAsync(cancellationToken);

            return new Response<bool>(true, "Đổi mật khẩu thành công.");
        }
    }
}
