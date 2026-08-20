using FluentAssertions;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Features.DonNghi.Commands.DeleteDonNghi;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;

namespace PayrollManagementSystem.UnitTests.Application.Features.DonNghi.Commands.DeleteDonNghi
{
    public class DeleteDonNghiCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly DeleteDonNghiCommandHandler _handler;

        public DeleteDonNghiCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new DeleteDonNghiCommandHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_NotFound_ThrowsApiException()
        {
            var command = new DeleteDonNghiCommand { Id = Guid.NewGuid() };
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Không tìm thấy đơn nghỉ");
        }

        [Fact]
        public async Task Handle_NotChoDuyet_ThrowsApiException()
        {
            var donNghi = new Domain.Models.DonNghi { Id = Guid.NewGuid(), CccdNhanVien = "001", TrangThai = TrangThaiDonNghi.DA_DUYET, LyDo = "Test" };
            _context.DonNghis.Add(donNghi);
            await _context.SaveChangesAsync();

            var command = new DeleteDonNghiCommand { Id = donNghi.Id };
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Chỉ có thể xóa đơn đang ở trạng thái 'Chờ duyệt'");
        }

        [Fact]
        public async Task Handle_ValidRequest_SoftRemovesDonNghi()
        {
            var donNghi = new Domain.Models.DonNghi { Id = Guid.NewGuid(), CccdNhanVien = "001", TrangThai = TrangThaiDonNghi.CHO_DUYET, LyDo = "Test" };
            _context.DonNghis.Add(donNghi);
            await _context.SaveChangesAsync();

            var command = new DeleteDonNghiCommand { Id = donNghi.Id };
            var result = await _handler.Handle(command, CancellationToken.None);

            result.Succeeded.Should().BeTrue();

            var dbDonNghi = await _context.DonNghis.FindAsync(donNghi.Id);
            dbDonNghi.Should().NotBeNull();
            dbDonNghi!.IsDeleted.Should().BeTrue();
        }
    }
}
