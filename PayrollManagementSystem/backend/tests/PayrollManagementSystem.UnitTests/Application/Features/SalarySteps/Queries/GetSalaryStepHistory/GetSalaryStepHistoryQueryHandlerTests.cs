using FluentAssertions;
using PayrollManagementSystem.Application.Features.SalarySteps.Queries.GetSalaryStepHistory;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;

namespace PayrollManagementSystem.UnitTests.Application.Features.SalarySteps.Queries.GetSalaryStepHistory
{
    public class GetSalaryStepHistoryQueryHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly GetSalaryStepHistoryQueryHandler _handler;

        public GetSalaryStepHistoryQueryHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new GetSalaryStepHistoryQueryHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_ReturnsHistorySortedDesc()
        {
            var bl1 = new BacLuong
            {
                IdBacLuong = "BL01",
                IdNgachLuong = "NL01",
                TenBacLuong = "Bậc 1",
                LuongP1 = 5000000,
                NgayApDung = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30)),
                TrangThai = TrangThaiBacLuong.HET_HIEU_LUC
            };
            var bl2 = new BacLuong
            {
                IdBacLuong = "BL02",
                IdNgachLuong = "NL01",
                TenBacLuong = "Bậc 1",
                LuongP1 = 6000000,
                NgayApDung = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10)),
                TrangThai = TrangThaiBacLuong.HIEU_LUC
            };
            var bl3 = new BacLuong
            {
                IdBacLuong = "BL03",
                IdNgachLuong = "NL01", // Different step
                TenBacLuong = "Bậc 2",
                LuongP1 = 7000000,
                NgayApDung = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10)),
                TrangThai = TrangThaiBacLuong.HIEU_LUC
            };
            _context.BacLuongs.AddRange(bl1, bl2, bl3);
            await _context.SaveChangesAsync();

            var query = new GetSalaryStepHistoryQuery { JobGradeId = "NL01", StepName = "Bậc 1" };

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            result.Data.Should().HaveCount(2);

            var list = result.Data.ToList();
            list[0].Id.Should().Be("BL02"); // Newer first
            list[1].Id.Should().Be("BL01");
        }
    }
}
