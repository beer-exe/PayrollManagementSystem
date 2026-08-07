using FluentAssertions;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Features.Payroll.Commands.CalculatePayroll;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.Payroll.Commands.CalculatePayroll
{
    public class CalculatePayrollCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly CalculatePayrollCommandHandler _handler;

        public CalculatePayrollCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new CalculatePayrollCommandHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_UnclosedPreviousPayroll_ThrowsApiException()
        {
            // Arrange
            _context.KyLuongs.Add(new KyLuong
            {
                IdKyLuong = Guid.NewGuid(),
                Thang = 5,
                Nam = 2026,
                TrangThai = TrangThaiKyLuong.CHUA_CHOT,
                TenKyLuong = "Tháng 5/2026",
                NgayBatDau = new DateOnly(2026, 5, 1),
                NgayKetThuc = new DateOnly(2026, 5, 31)
            });
            await _context.SaveChangesAsync();

            var command = new CalculatePayrollCommand { Thang = 6, Nam = 2026 };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("chưa được chốt");
        }

        [Fact]
        public async Task Handle_CurrentPayrollAlreadyClosed_ThrowsApiException()
        {
            // Arrange
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

            var command = new CalculatePayrollCommand { Thang = 6, Nam = 2026 };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("đã chốt hoặc đã thanh toán");
        }

        [Fact]
        public async Task Handle_NoActiveEmployees_SucceedsAndCreatesKyLuong()
        {
            // Arrange
            var command = new CalculatePayrollCommand { Thang = 6, Nam = 2026 };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            result.Message.Should().Contain("Tính lương thành công");

            var kyLuong = _context.KyLuongs.FirstOrDefault(x => x.Thang == 6 && x.Nam == 2026);
            kyLuong.Should().NotBeNull();
            kyLuong!.TrangThai.Should().Be(TrangThaiKyLuong.CHUA_CHOT);

            var bangLuongs = _context.BangLuongs.Where(x => x.Thang == 6 && x.Nam == 2026).ToList();
            bangLuongs.Should().BeEmpty();
        }
    }
}
