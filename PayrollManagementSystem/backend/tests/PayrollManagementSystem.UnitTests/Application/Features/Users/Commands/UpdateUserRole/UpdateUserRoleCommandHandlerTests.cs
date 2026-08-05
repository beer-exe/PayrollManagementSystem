using FluentAssertions;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Features.Users.Commands.UpdateUserRole;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.Users.Commands.UpdateUserRole
{
    public class UpdateUserRoleCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly UpdateUserRoleCommandHandler _handler;

        public UpdateUserRoleCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new UpdateUserRoleCommandHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_UserNotFound_ThrowsApiException()
        {
            var command = new UpdateUserRoleCommand { IdTaiKhoan = Guid.NewGuid(), IdVaiTroMoi = Guid.NewGuid() };

            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Tài khoản không tồn tại");
        }

        [Fact]
        public async Task Handle_RoleNotFound_ThrowsApiException()
        {
            var userId = Guid.NewGuid();
            _context.TaiKhoans.Add(new TaiKhoan { IdTaiKhoan = userId, TenTaiKhoan = "test", MatKhauHash = "hash" });
            await _context.SaveChangesAsync();

            var command = new UpdateUserRoleCommand { IdTaiKhoan = userId, IdVaiTroMoi = Guid.NewGuid() };

            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Vai trò không hợp lệ");
        }

        [Fact]
        public async Task Handle_ValidRequest_UpdatesRole()
        {
            var userId = Guid.NewGuid();
            var roleId1 = Guid.NewGuid();
            var roleId2 = Guid.NewGuid();
            var account = new TaiKhoan { IdTaiKhoan = userId, TenTaiKhoan = "test", MatKhauHash = "hash", IdVaiTro = roleId1 };
            _context.TaiKhoans.Add(account);
            _context.VaiTros.Add(new VaiTro { IdVaiTro = roleId2, TenVaiTro = "Admin" });
            await _context.SaveChangesAsync();

            var command = new UpdateUserRoleCommand { IdTaiKhoan = userId, IdVaiTroMoi = roleId2 };
            var result = await _handler.Handle(command, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            account.IdVaiTro.Should().Be(roleId2);
        }
    }
}
