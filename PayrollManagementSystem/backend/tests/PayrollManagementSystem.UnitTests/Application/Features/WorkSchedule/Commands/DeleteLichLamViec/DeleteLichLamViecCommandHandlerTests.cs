using FluentAssertions;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Features.WorkSchedule.Commands.DeleteLichLamViec;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.WorkSchedule.Commands.DeleteLichLamViec
{
    public class DeleteLichLamViecCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly DeleteLichLamViecCommandHandler _handler;

        public DeleteLichLamViecCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new DeleteLichLamViecCommandHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_ScheduleNotFound_ThrowsApiException()
        {
            var command = new DeleteLichLamViecCommand { IdLich = Guid.NewGuid() };

            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Không tìm thấy lịch làm việc");
        }

        [Fact]
        public async Task Handle_ValidRequest_DeletesScheduleAndDetails()
        {
            var lichId = Guid.NewGuid();
            var lich = new LichLamViec
            {
                IdLich = lichId,
                Nam = 2024,
                ChiTietLichLamViecs = new List<ChiTietLichLamViec>
                {
                    new ChiTietLichLamViec { Id = Guid.NewGuid(), IdLich = lichId, Ngay = new DateOnly(2024, 1, 1), Thu = "Hai" },
                    new ChiTietLichLamViec { Id = Guid.NewGuid(), IdLich = lichId, Ngay = new DateOnly(2024, 1, 2), Thu = "Ba" }
                }
            };
            _context.LichLamViecs.Add(lich);
            await _context.SaveChangesAsync();

            var command = new DeleteLichLamViecCommand { IdLich = lichId };
            var result = await _handler.Handle(command, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            result.Message.Should().Contain("Xóa lịch làm việc năm 2024 thành công");

            var deletedLich = await _context.LichLamViecs.FindAsync(lichId);
            deletedLich!.IsDeleted.Should().BeTrue();

            var deletedDetails = _context.ChiTietLichLamViecs.Where(c => c.IdLich == lichId).ToList();
            deletedDetails.Should().AllSatisfy(c => c.IsDeleted.Should().BeTrue());
        }
    }
}
