using FluentAssertions;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Features.DonNghi.Commands.TuChoiDonNghi;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.DonNghi.Commands.TuChoiDonNghi
{
    public class TuChoiDonNghiCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly TuChoiDonNghiCommandHandler _handler;

        public TuChoiDonNghiCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new TuChoiDonNghiCommandHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_DonNghiNotFound_ThrowsApiException()
        {
            var command = new TuChoiDonNghiCommand { Id = Guid.NewGuid(), CccdNguoiDuyet = "ADMIN", LyDoTuChoi = "Test" };
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Không tìm thấy đơn nghỉ");
        }

        [Fact]
        public async Task Handle_NotChoDuyet_ThrowsApiException()
        {
            var donNghi = new Domain.Models.DonNghi { Id = Guid.NewGuid(), CccdNhanVien = "001", TrangThai = TrangThaiDonNghi.DA_DUYET, LyDo = "Test" };
            _context.DonNghis.Add(donNghi);
            await _context.SaveChangesAsync();

            var command = new TuChoiDonNghiCommand { Id = donNghi.Id, CccdNguoiDuyet = "ADMIN", LyDoTuChoi = "Test" };
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Chỉ có thể từ chối đơn đang ở trạng thái 'Chờ duyệt'");
        }

        [Fact]
        public async Task Handle_ValidRequest_UpdatesStatusToTuChoi()
        {
            // Arrange
            var donNghi = new Domain.Models.DonNghi { Id = Guid.NewGuid(), CccdNhanVien = "001", TrangThai = TrangThaiDonNghi.CHO_DUYET, LyDo = "Test" };
            _context.DonNghis.Add(donNghi);
            await _context.SaveChangesAsync();

            var command = new TuChoiDonNghiCommand { Id = donNghi.Id, CccdNguoiDuyet = "ADMIN", LyDoTuChoi = "Lý do từ chối" };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            
            var dbDonNghi = await _context.DonNghis.FindAsync(donNghi.Id);
            dbDonNghi!.TrangThai.Should().Be(TrangThaiDonNghi.TU_CHOI);
            dbDonNghi.LyDoTuChoi.Should().Be("Lý do từ chối");
            dbDonNghi.CccdNguoiDuyet.Should().Be("ADMIN");
        }
    }
}
