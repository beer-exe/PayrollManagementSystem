using FluentAssertions;
using PayrollManagementSystem.Application.Features.WorkShifts.Queries.GetCaLamViecs;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;

namespace PayrollManagementSystem.UnitTests.Application.Features.WorkShifts.Queries.GetCaLamViecs
{
    public class GetCaLamViecsQueryHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly GetCaLamViecsQueryHandler _handler;

        public GetCaLamViecsQueryHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new GetCaLamViecsQueryHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsAllShifts_OrderedByStartTime()
        {
            var shift1 = new CaLamViec { Id = Guid.NewGuid(), TenCa = "Ca 2", GioBatDau = new TimeSpan(13, 0, 0), GioKetThuc = new TimeSpan(17, 0, 0) };
            var shift2 = new CaLamViec { Id = Guid.NewGuid(), TenCa = "Ca 1", GioBatDau = new TimeSpan(8, 0, 0), GioKetThuc = new TimeSpan(12, 0, 0) };

            _context.CaLamViecs.AddRange(shift1, shift2);
            await _context.SaveChangesAsync();

            var query = new GetCaLamViecsQuery();
            var result = await _handler.Handle(query, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            result.Data.Should().HaveCount(2);

            // Should be ordered by GioBatDau (Ca 1 first, then Ca 2)
            result.Data[0].TenCa.Should().Be("Ca 1");
            result.Data[1].TenCa.Should().Be("Ca 2");
        }

        [Fact]
        public async Task Handle_WithStatusFilter_ReturnsFilteredShifts()
        {
            var shift1 = new CaLamViec { Id = Guid.NewGuid(), TenCa = "Ca 1", TrangThai = true, GioBatDau = new TimeSpan(8, 0, 0) };
            var shift2 = new CaLamViec { Id = Guid.NewGuid(), TenCa = "Ca 2", TrangThai = false, GioBatDau = new TimeSpan(9, 0, 0) };

            _context.CaLamViecs.AddRange(shift1, shift2);
            await _context.SaveChangesAsync();

            var query = new GetCaLamViecsQuery { TrangThai = true };
            var result = await _handler.Handle(query, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            result.Data.Should().HaveCount(1);
            result.Data[0].TenCa.Should().Be("Ca 1");
        }
    }
}
