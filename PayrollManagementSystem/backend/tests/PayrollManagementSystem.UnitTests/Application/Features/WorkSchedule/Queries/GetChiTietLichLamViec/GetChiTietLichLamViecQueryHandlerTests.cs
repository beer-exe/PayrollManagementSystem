using FluentAssertions;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Features.WorkSchedule.Queries.GetChiTietLichLamViec;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;

namespace PayrollManagementSystem.UnitTests.Application.Features.WorkSchedule.Queries.GetChiTietLichLamViec
{
    public class GetChiTietLichLamViecQueryHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly GetChiTietLichLamViecQueryHandler _handler;

        public GetChiTietLichLamViecQueryHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new GetChiTietLichLamViecQueryHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_ScheduleNotFound_ThrowsApiException()
        {
            var query = new GetChiTietLichLamViecQuery { IdLich = Guid.NewGuid(), Thang = 1, PageNumber = 1, PageSize = 10 };

            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(query, CancellationToken.None));
            exception.Message.Should().Contain("Không tìm thấy lịch làm việc");
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsPagedDetailsForMonth()
        {
            var lichId = Guid.NewGuid();
            var lich = new LichLamViec { IdLich = lichId, Nam = 2024 };
            _context.LichLamViecs.Add(lich);

            // Month 1 details
            _context.ChiTietLichLamViecs.Add(new ChiTietLichLamViec { Id = Guid.NewGuid(), IdLich = lichId, Ngay = new DateOnly(2024, 1, 1), Thu = "Hai", LoaiNgay = LoaiNgay.NGAY_LAM_VIEC });
            _context.ChiTietLichLamViecs.Add(new ChiTietLichLamViec { Id = Guid.NewGuid(), IdLich = lichId, Ngay = new DateOnly(2024, 1, 2), Thu = "Ba", LoaiNgay = LoaiNgay.NGHI_CUOI_TUAN });

            // Month 2 details
            _context.ChiTietLichLamViecs.Add(new ChiTietLichLamViec { Id = Guid.NewGuid(), IdLich = lichId, Ngay = new DateOnly(2024, 2, 1), Thu = "Tư", LoaiNgay = LoaiNgay.NGAY_LAM_VIEC });

            await _context.SaveChangesAsync();

            var query = new GetChiTietLichLamViecQuery { IdLich = lichId, Thang = 1, PageNumber = 1, PageSize = 10 };
            var result = await _handler.Handle(query, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            result.Data.Should().HaveCount(2);
            result.TotalRecords.Should().Be(2);
            result.Data.Should().AllSatisfy(c => c.Ngay.Month.Should().Be(1));
        }
    }
}
