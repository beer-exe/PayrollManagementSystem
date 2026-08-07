using FluentAssertions;
using Moq;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.Users.Commands.ResetPassword;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.Users.Commands.ResetPassword
{
    public class ResetPasswordCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly ResetPasswordCommandHandler _handler;

        public ResetPasswordCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _emailServiceMock = new Mock<IEmailService>();
            _handler = new ResetPasswordCommandHandler(_context, _passwordHasherMock.Object, _emailServiceMock.Object);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_UserNotFound_ThrowsApiException()
        {
            var command = new ResetPasswordCommand { IdTaiKhoan = Guid.NewGuid(), NewPassword = "new" };

            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Tài khoản không tồn tại");
        }

        [Fact]
        public async Task Handle_ValidRequest_ResetsPassword()
        {
            var userId = Guid.NewGuid();
            _context.TaiKhoans.Add(new TaiKhoan { IdTaiKhoan = userId, TenTaiKhoan = "test", MatKhauHash = "old_hash", DangNhapLanDau = false });
            await _context.SaveChangesAsync();

            _passwordHasherMock.Setup(x => x.HashPasswordEnhanced("new")).Returns("new_hash");

            var command = new ResetPasswordCommand { IdTaiKhoan = userId, NewPassword = "new" };
            var result = await _handler.Handle(command, CancellationToken.None);

            result.Succeeded.Should().BeTrue();

            var updated = await _context.TaiKhoans.FindAsync(userId);
            updated!.MatKhauHash.Should().Be("new_hash");
            updated.DangNhapLanDau.Should().BeTrue();
        }
    }
}
