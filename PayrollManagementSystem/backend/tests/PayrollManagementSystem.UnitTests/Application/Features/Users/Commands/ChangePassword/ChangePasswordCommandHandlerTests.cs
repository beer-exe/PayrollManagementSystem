using FluentAssertions;
using Moq;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.Users.Commands.ChangePassword;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;

namespace PayrollManagementSystem.UnitTests.Application.Features.Users.Commands.ChangePassword
{
    public class ChangePasswordCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly ChangePasswordCommandHandler _handler;

        public ChangePasswordCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _handler = new ChangePasswordCommandHandler(_context, _passwordHasherMock.Object, _currentUserServiceMock.Object);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_UserNotFoundInSession_ThrowsApiException()
        {
            _currentUserServiceMock.Setup(x => x.UserId).Returns((Guid?)null);
            var command = new ChangePasswordCommand { OldPassword = "old", NewPassword = "new" };

            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Không tìm thấy thông tin phiên đăng nhập");
        }

        [Fact]
        public async Task Handle_UserNotFoundInDb_ThrowsApiException()
        {
            _currentUserServiceMock.Setup(x => x.UserId).Returns(Guid.NewGuid());
            var command = new ChangePasswordCommand { OldPassword = "old", NewPassword = "new" };

            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Tài khoản không tồn tại");
        }

        [Fact]
        public async Task Handle_OldPasswordIncorrect_ThrowsApiException()
        {
            var userId = Guid.NewGuid();
            _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
            _context.TaiKhoans.Add(new TaiKhoan { IdTaiKhoan = userId, TenTaiKhoan = "test", MatKhauHash = "hash1" });
            await _context.SaveChangesAsync();

            _passwordHasherMock.Setup(x => x.VerifyPasswordEnhanced("old", "hash1")).Returns(false);

            var command = new ChangePasswordCommand { OldPassword = "old", NewPassword = "new" };

            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Mật khẩu cũ không chính xác");
        }

        [Fact]
        public async Task Handle_ValidRequest_UpdatesPassword()
        {
            var userId = Guid.NewGuid();
            _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
            _context.TaiKhoans.Add(new TaiKhoan { IdTaiKhoan = userId, TenTaiKhoan = "test", MatKhauHash = "hash1" });
            await _context.SaveChangesAsync();

            _passwordHasherMock.Setup(x => x.VerifyPasswordEnhanced("old", "hash1")).Returns(true);
            _passwordHasherMock.Setup(x => x.HashPasswordEnhanced("new")).Returns("hash2");

            var command = new ChangePasswordCommand { OldPassword = "old", NewPassword = "new" };

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Succeeded.Should().BeTrue();

            var updated = await _context.TaiKhoans.FindAsync(userId);
            updated!.MatKhauHash.Should().Be("hash2");
        }
    }
}
