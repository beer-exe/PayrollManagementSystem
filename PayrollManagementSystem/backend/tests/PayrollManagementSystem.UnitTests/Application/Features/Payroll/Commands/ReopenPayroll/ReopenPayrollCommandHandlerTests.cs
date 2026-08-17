using FluentAssertions;
using Moq;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.Payroll.Commands.ReopenPayroll;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.Payroll.Commands.ReopenPayroll
{
    public class ReopenPayrollCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<IHrAuthorizationService> _hrAuthorizationServiceMock;
        private readonly ReopenPayrollCommandHandler _handler;

        public ReopenPayrollCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _hrAuthorizationServiceMock = new Mock<IHrAuthorizationService>();

            _currentUserServiceMock.Setup(x => x.UserId).Returns(Guid.NewGuid());
            _hrAuthorizationServiceMock.Setup(x => x.IsHrManagerAsync(It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _handler = new ReopenPayrollCommandHandler(
                _context, 
                _currentUserServiceMock.Object, 
                _hrAuthorizationServiceMock.Object);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_NotHrManager_ThrowsApiException()
        {
            _hrAuthorizationServiceMock.Setup(x => x.IsHrManagerAsync(It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var command = new ReopenPayrollCommand { Thang = 6, Nam = 2026, LyDo = "Tính bổ sung" };
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Chỉ HR cấp quản lý mới có quyền mở chốt kỳ lương");
        }

        [Fact]
        public async Task Handle_PayrollNotFound_ThrowsApiException()
        {
            var command = new ReopenPayrollCommand { Thang = 6, Nam = 2026, LyDo = "Tính bổ sung" };
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("chưa được tạo");
        }

        [Fact]
        public async Task Handle_PayrollNotClosed_ThrowsApiException()
        {
            _context.KyLuongs.Add(new KyLuong
            {
                IdKyLuong = Guid.NewGuid(),
                Thang = 6,
                Nam = 2026,
                TrangThai = TrangThaiKyLuong.CHUA_CHOT,
                TenKyLuong = "Tháng 6/2026",
                NgayBatDau = new DateOnly(2026, 6, 1),
                NgayKetThuc = new DateOnly(2026, 6, 30)
            });
            await _context.SaveChangesAsync();

            var command = new ReopenPayrollCommand { Thang = 6, Nam = 2026, LyDo = "Tính bổ sung" };
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("hiện chưa chốt");
        }

        [Fact]
        public async Task Handle_ValidRequest_ReopensPayroll()
        {
            var kyLuongId = Guid.NewGuid();
            _context.KyLuongs.Add(new KyLuong
            {
                IdKyLuong = kyLuongId,
                Thang = 6,
                Nam = 2026,
                TrangThai = TrangThaiKyLuong.DA_CHOT,
                TenKyLuong = "Tháng 6/2026",
                NgayBatDau = new DateOnly(2026, 6, 1),
                NgayKetThuc = new DateOnly(2026, 6, 30)
            });
            await _context.SaveChangesAsync();

            var command = new ReopenPayrollCommand { Thang = 6, Nam = 2026, LyDo = "Cần tính bù công nửa tháng sau" };
            var result = await _handler.Handle(command, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            result.Message.Should().Contain("thành công");

            var kyLuong = await _context.KyLuongs.FindAsync(kyLuongId);
            kyLuong!.TrangThai.Should().Be(TrangThaiKyLuong.CHUA_CHOT);
        }
    }
}
