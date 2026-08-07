using FluentAssertions;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Features.DonNghi.Queries.CalculateSoNgayNghi;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.DonNghi.Queries.CalculateSoNgayNghi
{
    public class CalculateSoNgayNghiQueryHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly CalculateSoNgayNghiQueryHandler _handler;

        public CalculateSoNgayNghiQueryHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new CalculateSoNgayNghiQueryHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_InvalidDates_ThrowsApiException()
        {
            var query = new CalculateSoNgayNghiQuery 
            { 
                NgayBatDau = new DateOnly(2025, 1, 2), 
                NgayKetThuc = new DateOnly(2025, 1, 1), 
                LoaiNghi = "NGHI_PHEP_NAM" 
            };
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(query, CancellationToken.None));
            exception.Message.Should().Contain("nhỏ hơn ngày bắt đầu");
        }

        [Fact]
        public async Task Handle_DifferentYears_ThrowsApiException()
        {
            var query = new CalculateSoNgayNghiQuery 
            { 
                NgayBatDau = new DateOnly(2025, 12, 31), 
                NgayKetThuc = new DateOnly(2026, 1, 1), 
                LoaiNghi = "NGHI_PHEP_NAM" 
            };
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(query, CancellationToken.None));
            exception.Message.Should().Contain("cùng nằm trong một năm");
        }

        [Fact]
        public async Task Handle_NoLichLamViec_ThrowsApiException()
        {
            var query = new CalculateSoNgayNghiQuery 
            { 
                NgayBatDau = new DateOnly(2025, 1, 1), 
                NgayKetThuc = new DateOnly(2025, 1, 2), 
                LoaiNghi = "NGHI_PHEP_NAM" 
            };
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(query, CancellationToken.None));
            exception.Message.Should().Contain("Chưa có lịch làm việc nào được tạo");
        }

        [Fact]
        public async Task Handle_NghiPhepNam_CountsOnlyWorkingDays()
        {
            // Arrange
            var lichId = Guid.NewGuid();
            _context.LichLamViecs.Add(new LichLamViec { IdLich = lichId, Nam = 2025 });
            _context.ChiTietLichLamViecs.AddRange(
                new ChiTietLichLamViec { IdLich = lichId, Ngay = new DateOnly(2025, 1, 1), Thu = "T4", LoaiNgay = LoaiNgay.NGAY_LAM_VIEC },
                new ChiTietLichLamViec { IdLich = lichId, Ngay = new DateOnly(2025, 1, 2), Thu = "T5", LoaiNgay = LoaiNgay.NGHI_CUOI_TUAN }
            );
            await _context.SaveChangesAsync();

            var query = new CalculateSoNgayNghiQuery 
            { 
                NgayBatDau = new DateOnly(2025, 1, 1), 
                NgayKetThuc = new DateOnly(2025, 1, 2), 
                LoaiNghi = "NGHI_PHEP_NAM" 
            };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            result.Data.Should().Be(1m); // Only 1 working day
        }

        [Fact]
        public async Task Handle_NghiThaiSan_CountsAllCalendarDays()
        {
            // Arrange
            _context.LichLamViecs.Add(new LichLamViec { IdLich = Guid.NewGuid(), Nam = 2025 });
            await _context.SaveChangesAsync();

            var query = new CalculateSoNgayNghiQuery 
            { 
                NgayBatDau = new DateOnly(2025, 1, 1), 
                NgayKetThuc = new DateOnly(2025, 1, 5), 
                LoaiNghi = "NGHI_THAI_SAN" 
            };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            result.Data.Should().Be(5m); // 5 calendar days
        }
    }
}
