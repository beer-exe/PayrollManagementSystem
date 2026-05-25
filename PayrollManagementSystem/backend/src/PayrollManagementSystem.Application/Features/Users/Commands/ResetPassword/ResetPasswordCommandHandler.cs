using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Users.Commands.ResetPassword
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IEmailService _emailService;

        public ResetPasswordCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher, IEmailService emailService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _emailService = emailService;
        }

        public async Task<Response<bool>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var taiKhoan = await _context.TaiKhoans
                .Include(t => t.NhanVien)
                .FirstOrDefaultAsync(t => t.IdTaiKhoan == request.IdTaiKhoan, cancellationToken);

            if (taiKhoan == null) throw new ApiException("Tài khoản không tồn tại.");

            taiKhoan.MatKhauHash = _passwordHasher.HashPasswordEnhanced(request.NewPassword);
            taiKhoan.DangNhapLanDau = true;

            await _context.SaveChangesAsync(cancellationToken);

            //if (taiKhoan.NhanVien != null && !string.IsNullOrEmpty(taiKhoan.NhanVien.Email))
            //{
            //    string emailBody = $"Chào {taiKhoan.NhanVien.HoTen},<br/><br/>Mật khẩu tài khoản của bạn đã được Admin đặt lại thành công.<br/>Mật khẩu mới của bạn là: <b>{request.NewPassword}</b><br/>Vui lòng đăng nhập và đổi mật khẩu ngay lập tức.";
            //    _ = _emailService.SendAsync(taiKhoan.NhanVien.Email, "Thông báo đặt lại mật khẩu", emailBody);
            //}

            return new Response<bool>(true, "Đặt lại mật khẩu thành công.");
        }
    }
}
