using FluentAssertions;
using PayrollManagementSystem.Application.Features.Positions.Queries.GetPositions;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.Positions.Queries.GetPositions
{
    public class GetPositionsQueryHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly GetPositionsQueryHandler _handler;

        public GetPositionsQueryHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new GetPositionsQueryHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_ReturnsFilteredList()
        {
            // Arrange
            _context.PhongBans.Add(new PhongBan { IdPb = "PB01", TenPb = "IT" });
            _context.PhongBans.Add(new PhongBan { IdPb = "PB02", TenPb = "HR" });
            _context.ChucVus.Add(new ChucVu { IdChucVu = "CV01", TenChucVu = "Developer", IdPhongBan = "PB01", TrangThai = TrangThaiChucVu.HOAT_DONG });
            _context.ChucVus.Add(new ChucVu { IdChucVu = "CV02", TenChucVu = "Manager", IdPhongBan = "PB02", TrangThai = TrangThaiChucVu.NGUNG_HOAT_DONG });
            _context.ChucVus.Add(new ChucVu { IdChucVu = "CV03", TenChucVu = "DevOps", IdPhongBan = "PB01", TrangThai = TrangThaiChucVu.HOAT_DONG });
            await _context.SaveChangesAsync();

            // Act 1: Get all
            var result1 = await _handler.Handle(new GetPositionsQuery(), CancellationToken.None);
            result1.Data.Should().HaveCount(3);

            // Act 2: Filter by ID/Name
            var result2 = await _handler.Handle(new GetPositionsQuery { SearchTerm = "dev" }, CancellationToken.None);
            result2.Data.Should().HaveCount(2);

            // Act 3: Filter by Department
            var result3 = await _handler.Handle(new GetPositionsQuery { IdPhongBan = "PB02" }, CancellationToken.None);
            result3.Data.Should().HaveCount(1);
            result3.Data.First().IdChucVu.Should().Be("CV02");

            // Act 4: Filter by Status
            var result4 = await _handler.Handle(new GetPositionsQuery { TrangThai = TrangThaiChucVu.NGUNG_HOAT_DONG }, CancellationToken.None);
            result4.Data.Should().HaveCount(1);
            result4.Data.First().IdChucVu.Should().Be("CV02");
        }
    }
}
