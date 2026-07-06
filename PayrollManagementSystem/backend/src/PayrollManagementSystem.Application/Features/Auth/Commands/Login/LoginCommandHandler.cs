using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.Auth.DTOs;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;

namespace PayrollManagementSystem.Application.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Response<AuthResponseDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public LoginCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<Response<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            TaiKhoan? taiKhoan = _context.TaiKhoans
                .Include(t => t.NhanVien)
                .Include(t => t.VaiTro)
                .FirstOrDefault(t => t.TenTaiKhoan == request.TenTaiKhoan);

            if (taiKhoan == null || !_passwordHasher.VerifyPasswordEnhanced(request.MatKhau, taiKhoan.MatKhauHash))
            {
                throw new ApiException("Tài khoản hoặc mật khẩu không chính xác.");
            }

            if (taiKhoan.TrangThai == TrangThaiTaiKhoan.KHOA)
            {
                throw new ApiException("Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên.");
            }

            string? accessToken = _jwtTokenGenerator.GenerateAccessToken(taiKhoan);
            string? refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

            taiKhoan.RefreshToken = refreshToken;
            taiKhoan.RefreshTokenExpiryTime = DateTime.SpecifyKind(DateTime.UtcNow.AddDays(7), DateTimeKind.Unspecified);
            await _context.SaveChangesAsync(cancellationToken);

            bool hasDirectReports = false;
            if (taiKhoan.NhanVien != null && !string.IsNullOrEmpty(taiKhoan.NhanVien.Cccd))
            {
                hasDirectReports = await _context.NhanViens.AnyAsync(nv => nv.CccdNguoiQuanLy == taiKhoan.NhanVien.Cccd, cancellationToken);
            }

            AuthResponseDto? responseData = new AuthResponseDto
            {
                UserId = taiKhoan.IdTaiKhoan.ToString(),
                FullName = taiKhoan.TenTaiKhoan,
                Email = taiKhoan.NhanVien?.Email ?? string.Empty,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                HasDirectReports = hasDirectReports
            };

            return new Response<AuthResponseDto>(responseData, "Đăng nhập thành công.");
        }
    }
}
