using FluentAssertions;
using PayrollManagementSystem.Application.Features.KhoanKhauTru.Queries.GetKhoanKhauTruList;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.KhoanKhauTru.Queries.GetKhoanKhauTruList
{
    public class GetKhoanKhauTruListQueryHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly GetKhoanKhauTruListQueryHandler _handler;

        public GetKhoanKhauTruListQueryHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new GetKhoanKhauTruListQueryHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_ReturnsAllEntities()
        {
            _context.KhoanKhauTrus.Add(new Domain.Models.KhoanKhauTru { IdKhoanKhauTru = Guid.NewGuid(), TenKhoanKhauTru = "A", IsActive = true, CreatedAt = DateTime.UtcNow.AddDays(-1) });
            _context.KhoanKhauTrus.Add(new Domain.Models.KhoanKhauTru { IdKhoanKhauTru = Guid.NewGuid(), TenKhoanKhauTru = "B", IsActive = false, CreatedAt = DateTime.UtcNow });
            await _context.SaveChangesAsync();

            var query = new GetKhoanKhauTruListQuery();
            var result = await _handler.Handle(query, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            result.Data.Should().HaveCount(2);
            result.Data.First().TenKhoanKhauTru.Should().Be("A"); // Sorted by CreatedAt
            result.Data.Last().TenKhoanKhauTru.Should().Be("B");
        }
        
        [Fact]
        public async Task Handle_WithIsActiveFilter_ReturnsFilteredEntities()
        {
            _context.KhoanKhauTrus.Add(new Domain.Models.KhoanKhauTru { IdKhoanKhauTru = Guid.NewGuid(), TenKhoanKhauTru = "A", IsActive = true });
            _context.KhoanKhauTrus.Add(new Domain.Models.KhoanKhauTru { IdKhoanKhauTru = Guid.NewGuid(), TenKhoanKhauTru = "B", IsActive = false });
            await _context.SaveChangesAsync();

            var query = new GetKhoanKhauTruListQuery { IsActive = true };
            var result = await _handler.Handle(query, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            result.Data.Should().HaveCount(1);
            result.Data.First().TenKhoanKhauTru.Should().Be("A");
        }
    }
}
