using PayrollManagementSystem.Domain.Models;
using System.Security.Claims;

namespace PayrollManagementSystem.Application.Common.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateAccessToken(TaiKhoan user);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
