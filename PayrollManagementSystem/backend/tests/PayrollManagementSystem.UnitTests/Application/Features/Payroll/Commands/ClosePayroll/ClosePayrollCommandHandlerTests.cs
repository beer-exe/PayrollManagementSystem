using FluentAssertions;
using PayrollManagementSystem.Application.Common.Exceptions;
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
        private readonly ClosePayrollCommandHandler _handler;

        public ClosePayrollCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new ClosePayrollCommandHandler(_context);
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
        public async Task Handle_ValidRequest_ClosesPayroll()
        {
            var kyLuongId = Guid.NewGuid();
            _context.KyLuongs.Add(new KyLuong
            {
                IdKyLuong = kyLuongId,
                Thang = 6,
                Nam = 2026,
                TrangThai = TrangThaiKyLuong.CHUA_CHOT,
                TenKyLuong = "Tháng 6/2026",
                NgayBatDau = new DateOnly(2026, 6, 1),
                NgayKetThuc = new DateOnly(2026, 6, 30)
            });
            
            _context.BangLuongs.Add(new BangLuong
            {
                IdBangLuong = Guid.NewGuid(),
                IdKyLuong = kyLuongId,
                CccdNhanVien = "001",
                Thang = 6,
                Nam = 2026,
                ChiTietKhauTru = "[]",
                ChiTietThue = "{}"
            });
            
            await _context.SaveChangesAsync();

            var command = new ClosePayrollCommand { Thang = 6, Nam = 2026 };
            var result = await _handler.Handle(command, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            result.Message.Should().Contain("thành công");

            var kyLuong = await _context.KyLuongs.FindAsync(kyLuongId);
            kyLuong!.TrangThai.Should().Be(TrangThaiKyLuong.DA_CHOT);
        }
    }
}
