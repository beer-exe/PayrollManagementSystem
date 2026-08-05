using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Features.DonNghi.Commands.UpdateNgayPhep;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.DonNghi.Commands.UpdateNgayPhep
{
    public class UpdateNgayPhepCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly UpdateNgayPhepCommandHandler _handler;

        public UpdateNgayPhepCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new UpdateNgayPhepCommandHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_NoLichLamViec_ThrowsApiException()
        {
            var command = new UpdateNgayPhepCommand { CccdNhanVien = "001", Nam = 2025, TongNgayPhep = 12 };
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Chưa có lịch làm việc nào được tạo");
        }

        [Fact]
        public async Task Handle_EmployeeNotFound_ThrowsApiException()
        {
            _context.LichLamViecs.Add(new LichLamViec { IdLich = Guid.NewGuid(), Nam = 2025 });
            await _context.SaveChangesAsync();

            var command = new UpdateNgayPhepCommand { CccdNhanVien = "001", Nam = 2025, TongNgayPhep = 12 };
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Nhân viên không tồn tại");
        }

        [Fact]
        public async Task Handle_ValidRequest_CreatesNgayPhep()
        {
            // Arrange
            _context.LichLamViecs.Add(new LichLamViec { IdLich = Guid.NewGuid(), Nam = 2025 });
            _context.NhanViens.Add(new NhanVien { Cccd = "001", HoTen = "Test NV" });
            await _context.SaveChangesAsync();

            var command = new UpdateNgayPhepCommand { CccdNhanVien = "001", Nam = 2025, TongNgayPhep = 12 };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            
            var dbNgayPhep = await _context.NgayPhepNhanViens.FirstOrDefaultAsync(n => n.CccdNhanVien == "001" && n.Nam == 2025);
            dbNgayPhep.Should().NotBeNull();
            dbNgayPhep!.TongNgayPhep.Should().Be(12);
        }
    }
}
