using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.Auth.DTOs;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;
using System.Security.Claims;

namespace PayrollManagementSystem.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Response<AuthResponseDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public RefreshTokenCommandHandler(IApplicationDbContext context, IJwtTokenGenerator jwtTokenGenerator)
        {
            _context = context;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<Response<AuthResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            ClaimsPrincipal principal = _jwtTokenGenerator.GetPrincipalFromExpiredToken(request.AccessToken);

            string? userIdStr = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid userId))
            {
                throw new ApiException("Access token không hợp lệ hoặc không chứa thông tin định danh.");
            }

            TaiKhoan? taiKhoan = await _context.TaiKhoans
                .Include(t => t.NhanVien)
                .Include(t => t.VaiTro)
                .FirstOrDefaultAsync(t => t.IdTaiKhoan == userId, cancellationToken);

            if (taiKhoan == null)
            {
                throw new ApiException("Tài khoản không tồn tại.");
            }

            if (taiKhoan.TrangThai == TrangThaiTaiKhoan.KHOA)
            {
                throw new ApiException("Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên.");
            }

            if (taiKhoan.RefreshToken != request.RefreshToken)
            {
                throw new ApiException("Refresh Token không hợp lệ.");
            }

            if (taiKhoan.RefreshTokenExpiryTime == null || taiKhoan.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                throw new ApiException("Refresh Token đã hết hạn. Vui lòng đăng nhập lại.");
            }

            string newAccessToken = _jwtTokenGenerator.GenerateAccessToken(taiKhoan);
            string newRefreshToken = _jwtTokenGenerator.GenerateRefreshToken();

            taiKhoan.RefreshToken = newRefreshToken;
            taiKhoan.RefreshTokenExpiryTime = DateTime.SpecifyKind(DateTime.UtcNow.AddDays(7), DateTimeKind.Unspecified);

            await _context.SaveChangesAsync(cancellationToken);

            AuthResponseDto responseData = new AuthResponseDto
            {
                UserId = taiKhoan.IdTaiKhoan.ToString(),
                FullName = taiKhoan.TenTaiKhoan,
                Email = taiKhoan.NhanVien?.Email ?? string.Empty,
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };

            return new Response<AuthResponseDto>(responseData, "Làm mới Token thành công.");
        }
    }
}