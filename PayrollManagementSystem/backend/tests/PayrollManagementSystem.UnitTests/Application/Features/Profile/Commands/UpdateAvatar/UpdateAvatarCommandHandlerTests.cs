using FluentAssertions;
using Moq;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.Profile.Commands.UpdateAvatar;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.Profile.Commands.UpdateAvatar
{
    public class UpdateAvatarCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly UpdateAvatarCommandHandler _handler;

        public UpdateAvatarCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _handler = new UpdateAvatarCommandHandler(_context, _currentUserServiceMock.Object);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_NoUserId_ThrowsApiException()
        {
            _currentUserServiceMock.Setup(x => x.UserId).Returns((Guid?)null);

            var command = new UpdateAvatarCommand { AvatarBase64 = "base64data" };
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Không tìm thấy thông tin phiên đăng nhập");
        }

        [Fact]
        public async Task Handle_AccountNotFound_ThrowsApiException()
        {
            _currentUserServiceMock.Setup(x => x.UserId).Returns(Guid.NewGuid());

            var command = new UpdateAvatarCommand { AvatarBase64 = "base64data" };
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Tài khoản không tồn tại");
        }

        [Fact]
        public async Task Handle_ValidRequest_UpdatesAvatar()
        {
            var userId = Guid.NewGuid();
            _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);

            _context.TaiKhoans.Add(new TaiKhoan
            {
                IdTaiKhoan = userId,
                TenTaiKhoan = "testuser",
                MatKhauHash = "hash",
                UserAvatar = "old_avatar"
            });
            await _context.SaveChangesAsync();

            var command = new UpdateAvatarCommand { AvatarBase64 = "new_avatar" };
            var result = await _handler.Handle(command, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            result.Data.Should().Be("new_avatar");

            var taiKhoan = await _context.TaiKhoans.FindAsync(userId);
            taiKhoan!.UserAvatar.Should().Be("new_avatar");
        }
    }
}
