using FluentAssertions;
using Moq;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.Users.Commands.ToggleUserStatus;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.Users.Commands.ToggleUserStatus
{
    public class ToggleUserStatusCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly ToggleUserStatusCommandHandler _handler;

        public ToggleUserStatusCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _emailServiceMock = new Mock<IEmailService>();
            _handler = new ToggleUserStatusCommandHandler(_context, _emailServiceMock.Object);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_UserNotFound_ThrowsApiException()
        {
            var command = new ToggleUserStatusCommand { IdTaiKhoan = Guid.NewGuid() };

            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Tài khoản không tồn tại");
        }

        [Fact]
        public async Task Handle_ValidRequest_TogglesStatus()
        {
            var userId = Guid.NewGuid();
            var account = new TaiKhoan { IdTaiKhoan = userId, TenTaiKhoan = "test", MatKhauHash = "hash", TrangThai = TrangThaiTaiKhoan.HOAT_DONG };
            _context.TaiKhoans.Add(account);
            await _context.SaveChangesAsync();

            var command = new ToggleUserStatusCommand { IdTaiKhoan = userId };

            // Toggle to KHOA
            var result1 = await _handler.Handle(command, CancellationToken.None);
            result1.Succeeded.Should().BeTrue();
            account.TrangThai.Should().Be(TrangThaiTaiKhoan.KHOA);

            // Toggle back to HOAT_DONG
            var result2 = await _handler.Handle(command, CancellationToken.None);
            result2.Succeeded.Should().BeTrue();
            account.TrangThai.Should().Be(TrangThaiTaiKhoan.HOAT_DONG);
        }
    }
}
