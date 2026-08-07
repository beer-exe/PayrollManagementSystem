using FluentAssertions;
using Moq;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.Auth.Commands.Login;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.Auth.Commands.Login
{
    public class LoginCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<IPasswordHasher> _mockPasswordHasher;
        private readonly Mock<IJwtTokenGenerator> _mockJwtTokenGenerator;
        private readonly LoginCommandHandler _handler;

        public LoginCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _mockPasswordHasher = new Mock<IPasswordHasher>();
            _mockJwtTokenGenerator = new Mock<IJwtTokenGenerator>();
            _handler = new LoginCommandHandler(_context, _mockPasswordHasher.Object, _mockJwtTokenGenerator.Object);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_ValidCredentials_ReturnsAuthResponse()
        {
            // Arrange
            var taiKhoan = new TaiKhoan
            {
                IdTaiKhoan = Guid.NewGuid(),
                TenTaiKhoan = "admin",
                MatKhauHash = "hashed_password",
                TrangThai = TrangThaiTaiKhoan.HOAT_DONG,
                IdVaiTro = Guid.NewGuid(),
                NhanVien = new NhanVien { Cccd = "012345678912", Email = "admin@test.com", HoTen = "Admin User" }
            };
            _context.TaiKhoans.Add(taiKhoan);
            await _context.SaveChangesAsync();

            var command = new LoginCommand { TenTaiKhoan = "admin", MatKhau = "password123" };
            
            _mockPasswordHasher.Setup(x => x.VerifyPasswordEnhanced("password123", "hashed_password")).Returns(true);
            _mockJwtTokenGenerator.Setup(x => x.GenerateAccessToken(It.IsAny<TaiKhoan>())).Returns("access_token");
            _mockJwtTokenGenerator.Setup(x => x.GenerateRefreshToken()).Returns("refresh_token");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Succeeded.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.AccessToken.Should().Be("access_token");
            result.Data.RefreshToken.Should().Be("refresh_token");
            result.Data.Email.Should().Be("admin@test.com");
            
            // Verify db updated with refresh token
            var dbAccount = await _context.TaiKhoans.FindAsync(taiKhoan.IdTaiKhoan);
            dbAccount!.RefreshToken.Should().Be("refresh_token");
            dbAccount.RefreshTokenExpiryTime.Should().BeAfter(DateTime.UtcNow);
        }

        [Fact]
        public async Task Handle_InvalidUsername_ThrowsApiException()
        {
            // Arrange
            var command = new LoginCommand { TenTaiKhoan = "wrong_user", MatKhau = "password123" };

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ApiException>()
                .WithMessage("Tài khoản hoặc mật khẩu không chính xác.");
        }

        [Fact]
        public async Task Handle_InvalidPassword_ThrowsApiException()
        {
            // Arrange
            var taiKhoan = new TaiKhoan
            {
                IdTaiKhoan = Guid.NewGuid(),
                TenTaiKhoan = "admin",
                MatKhauHash = "hashed_password",
                TrangThai = TrangThaiTaiKhoan.HOAT_DONG
            };
            _context.TaiKhoans.Add(taiKhoan);
            await _context.SaveChangesAsync();

            var command = new LoginCommand { TenTaiKhoan = "admin", MatKhau = "wrong_password" };
            _mockPasswordHasher.Setup(x => x.VerifyPasswordEnhanced("wrong_password", "hashed_password")).Returns(false);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ApiException>()
                .WithMessage("Tài khoản hoặc mật khẩu không chính xác.");
        }

        [Fact]
        public async Task Handle_AccountLocked_ThrowsApiException()
        {
            // Arrange
            var taiKhoan = new TaiKhoan
            {
                IdTaiKhoan = Guid.NewGuid(),
                TenTaiKhoan = "admin",
                MatKhauHash = "hashed_password",
                TrangThai = TrangThaiTaiKhoan.KHOA
            };
            _context.TaiKhoans.Add(taiKhoan);
            await _context.SaveChangesAsync();

            var command = new LoginCommand { TenTaiKhoan = "admin", MatKhau = "password123" };
            _mockPasswordHasher.Setup(x => x.VerifyPasswordEnhanced("password123", "hashed_password")).Returns(true);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ApiException>()
                .WithMessage("Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên.");
        }
    }
}
