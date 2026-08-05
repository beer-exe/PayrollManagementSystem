using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using Moq;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.Auth.Commands.RefreshToken;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using System.Security.Claims;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<IJwtTokenGenerator> _mockJwtTokenGenerator;
        private readonly RefreshTokenCommandHandler _handler;

        public RefreshTokenCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _mockJwtTokenGenerator = new Mock<IJwtTokenGenerator>();
            _handler = new RefreshTokenCommandHandler(_context, _mockJwtTokenGenerator.Object);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_ValidToken_ReturnsNewTokens()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var taiKhoan = new TaiKhoan
            {
                IdTaiKhoan = userId,
                TenTaiKhoan = "admin",
                MatKhauHash = "hash",
                TrangThai = TrangThaiTaiKhoan.HOAT_DONG,
                RefreshToken = "old_refresh_token",
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1)
            };
            _context.TaiKhoans.Add(taiKhoan);
            await _context.SaveChangesAsync();

            var command = new RefreshTokenCommand { AccessToken = "old_access_token", RefreshToken = "old_refresh_token" };

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));
            
            _mockJwtTokenGenerator.Setup(x => x.GetPrincipalFromExpiredToken("old_access_token")).Returns(principal);
            _mockJwtTokenGenerator.Setup(x => x.GenerateAccessToken(It.IsAny<TaiKhoan>())).Returns("new_access_token");
            _mockJwtTokenGenerator.Setup(x => x.GenerateRefreshToken()).Returns("new_refresh_token");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Succeeded.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.AccessToken.Should().Be("new_access_token");
            result.Data.RefreshToken.Should().Be("new_refresh_token");
        }

        [Fact]
        public async Task Handle_InvalidAccessToken_ThrowsApiException()
        {
            // Arrange
            var command = new RefreshTokenCommand { AccessToken = "invalid_token", RefreshToken = "old_refresh_token" };
            
            _mockJwtTokenGenerator.Setup(x => x.GetPrincipalFromExpiredToken("invalid_token"))
                                  .Throws(new SecurityTokenException("Invalid token"));

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<SecurityTokenException>();
        }

        [Fact]
        public async Task Handle_ExpiredRefreshToken_ThrowsApiException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var taiKhoan = new TaiKhoan
            {
                IdTaiKhoan = userId,
                TenTaiKhoan = "admin",
                MatKhauHash = "hash",
                TrangThai = TrangThaiTaiKhoan.HOAT_DONG,
                RefreshToken = "old_refresh_token",
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(-1) // Expired
            };
            _context.TaiKhoans.Add(taiKhoan);
            await _context.SaveChangesAsync();

            var command = new RefreshTokenCommand { AccessToken = "old_access_token", RefreshToken = "old_refresh_token" };

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));
            
            _mockJwtTokenGenerator.Setup(x => x.GetPrincipalFromExpiredToken("old_access_token")).Returns(principal);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ApiException>()
                .WithMessage("Refresh Token đã hết hạn. Vui lòng đăng nhập lại.");
        }
    }
}
