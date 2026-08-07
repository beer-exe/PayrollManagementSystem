using FluentAssertions;
using PayrollManagementSystem.Application.Features.SalarySteps.Queries.GetActiveSalarySteps;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Extensions;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.SalarySteps.Queries.GetActiveSalarySteps
{
    public class GetActiveSalaryStepsQueryHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly GetActiveSalaryStepsQueryHandler _handler;

        public GetActiveSalaryStepsQueryHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new GetActiveSalaryStepsQueryHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_ReturnsActiveStepsAndMarksFutureAsChuaApDung()
        {
            var bl1 = new BacLuong
            {
                IdBacLuong = "BL01",
                IdNgachLuong = "NL01",
                TenBacLuong = "Bậc 1",
                LuongP1 = 5000000,
                NgayApDung = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10)),
                TrangThai = TrangThaiBacLuong.HIEU_LUC
            };
            var bl2 = new BacLuong
            {
                IdBacLuong = "BL02",
                IdNgachLuong = "NL01",
                TenBacLuong = "Bậc 2",
                LuongP1 = 6000000,
                NgayApDung = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)), // Future date
                TrangThai = TrangThaiBacLuong.HIEU_LUC
            };
            var bl3 = new BacLuong
            {
                IdBacLuong = "BL03",
                IdNgachLuong = "NL01",
                TenBacLuong = "Bậc 1_OLD",
                LuongP1 = 4000000,
                NgayApDung = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-20)),
                TrangThai = TrangThaiBacLuong.HET_HIEU_LUC // Should not be returned
            };
            var bl4 = new BacLuong
            {
                IdBacLuong = "BL04",
                IdNgachLuong = "NL02", // Different grade
                TenBacLuong = "Bậc 1",
                LuongP1 = 5500000,
                NgayApDung = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-5)),
                TrangThai = TrangThaiBacLuong.HIEU_LUC
            };

            _context.BacLuongs.AddRange(bl1, bl2, bl3, bl4);
            await _context.SaveChangesAsync();

            var query = new GetActiveSalaryStepsQuery { JobGradeId = "NL01" };

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            result.Data.Should().HaveCount(2);

            var list = result.Data.ToList();
            list.Should().ContainSingle(x => x.StepName == "Bậc 1");
            list.Should().ContainSingle(x => x.StepName == "Bậc 2");

            list.First(x => x.StepName == "Bậc 1").Status.Should().Be(TrangThaiBacLuong.HIEU_LUC.GetDescription());
            list.First(x => x.StepName == "Bậc 2").Status.Should().Be("CHUA_AP_DUNG");
        }
    }
}
