using FluentAssertions;
using Moq;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.Payroll.Commands.ClosePayroll;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.Payroll.Commands.ClosePayroll
{
    public class ClosePayrollCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<IHrAuthorizationService> _hrAuthorizationServiceMock;
        private readonly ClosePayrollCommandHandler _handler;

        public ClosePayrollCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _hrAuthorizationServiceMock = new Mock<IHrAuthorizationService>();
            _currentUserServiceMock.Setup(x => x.UserId).Returns(Guid.NewGuid());
            _hrAuthorizationServiceMock.Setup(x => x.IsHrManagerAsync(It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _handler = new ClosePayrollCommandHandler(
                _context, 
                _currentUserServiceMock.Object, 
                _hrAuthorizationServiceMock.Object);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_PayrollNotFound_ThrowsApiException()
        {
            var command = new ClosePayrollCommand { Thang = 6, Nam = 2026 };
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("chưa được tạo");
        }

        [Fact]
        public async Task Handle_PayrollAlreadyClosed_ThrowsApiException()
        {
            _context.KyLuongs.Add(new KyLuong
            {
                IdKyLuong = Guid.NewGuid(),
                Thang = 6,
                Nam = 2026,
                TrangThai = TrangThaiKyLuong.DA_CHOT,
                TenKyLuong = "Tháng 6/2026",
                NgayBatDau = new DateOnly(2026, 6, 1),
                NgayKetThuc = new DateOnly(2026, 6, 30)
            });
            await _context.SaveChangesAsync();

            var command = new ClosePayrollCommand { Thang = 6, Nam = 2026 };
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("đã được chốt hoặc thanh toán");
        }

        [Fact]
        public async Task Handle_NoBangLuong_ThrowsApiException()
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

            var command = new ClosePayrollCommand { Thang = 6, Nam = 2026 };
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("chưa có dữ liệu bảng lương");
        }

        [Fact]
        public async Task Handle_CloseBeforeMonthEnd_NotHrManager_ThrowsApiException()
        {
            var futureYear = DateTime.Today.Year + 1;
            var kyLuongId = Guid.NewGuid();
            _context.KyLuongs.Add(new KyLuong
            {
                IdKyLuong = kyLuongId,
                Thang = 12,
                Nam = futureYear,
                TrangThai = TrangThaiKyLuong.CHUA_CHOT,
                TenKyLuong = $"Tháng 12/{futureYear}",
                NgayBatDau = new DateOnly(futureYear, 12, 1),
                NgayKetThuc = new DateOnly(futureYear, 12, 31)
            });
            _context.BangLuongs.Add(new BangLuong
            {
                IdBangLuong = Guid.NewGuid(),
                IdKyLuong = kyLuongId,
                CccdNhanVien = "001",
                Thang = 12,
                Nam = futureYear,
                ChiTietKhauTru = "[]",
                ChiTietThue = "{}"
            });
            await _context.SaveChangesAsync();

            // Set user NOT HR Manager
            _hrAuthorizationServiceMock.Setup(x => x.IsHrManagerAsync(It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var command = new ClosePayrollCommand { Thang = 12, Nam = futureYear };
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Chỉ HR cấp quản lý mới có quyền chốt trước thời hạn");
        }

        [Fact]
        public async Task Handle_ValidRequest_ClosesPayroll()
        {
            var kyLuongId = Guid.NewGuid();
            _context.KyLuongs.Add(new KyLuong
            {
                IdKyLuong = kyLuongId,
                Thang = 1,
                Nam = 2020, // In the past
                TrangThai = TrangThaiKyLuong.CHUA_CHOT,
                TenKyLuong = "Tháng 1/2020",
                NgayBatDau = new DateOnly(2020, 1, 1),
                NgayKetThuc = new DateOnly(2020, 1, 31)
            });
            
            _context.BangLuongs.Add(new BangLuong
            {
                IdBangLuong = Guid.NewGuid(),
                IdKyLuong = kyLuongId,
                CccdNhanVien = "001",
                Thang = 1,
                Nam = 2020,
                ChiTietKhauTru = "[]",
                ChiTietThue = "{}"
            });
            
            await _context.SaveChangesAsync();

            var command = new ClosePayrollCommand { Thang = 1, Nam = 2020 };
            var result = await _handler.Handle(command, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            result.Message.Should().Contain("thành công");

            var kyLuong = await _context.KyLuongs.FindAsync(kyLuongId);
            kyLuong!.TrangThai.Should().Be(TrangThaiKyLuong.DA_CHOT);
        }
    }
}
