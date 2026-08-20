using FluentAssertions;
using PayrollManagementSystem.Application.Features.JobGrades.Queries.GetJobGrades;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;

namespace PayrollManagementSystem.UnitTests.Application.Features.JobGrades.Queries.GetJobGrades
{
    public class GetJobGradesQueryHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly GetJobGradesQueryHandler _handler;

        public GetJobGradesQueryHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new GetJobGradesQueryHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_ReturnsSortedJobGrades()
        {
            _context.NgachLuongs.Add(new NgachLuong { IdNgachLuong = "NL02", TenNgachLuong = "Ngạch B", TrangThai = TrangThaiNgachLuong.HOAT_DONG });
            _context.NgachLuongs.Add(new NgachLuong { IdNgachLuong = "NL01", TenNgachLuong = "Ngạch A", TrangThai = TrangThaiNgachLuong.HOAT_DONG });
            await _context.SaveChangesAsync();

            var query = new GetJobGradesQuery();
            var result = await _handler.Handle(query, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            result.Data.Should().HaveCount(2);
            result.Data.First().TenNgachLuong.Should().Be("Ngạch A");
            result.Data.Last().TenNgachLuong.Should().Be("Ngạch B");
        }
    }
}
