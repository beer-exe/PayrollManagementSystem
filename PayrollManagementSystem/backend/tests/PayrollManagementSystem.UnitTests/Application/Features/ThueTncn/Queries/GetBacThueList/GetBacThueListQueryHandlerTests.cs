using FluentAssertions;
using PayrollManagementSystem.Application.Features.ThueTncn.Queries.GetBacThueList;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;

namespace PayrollManagementSystem.UnitTests.Application.Features.ThueTncn.Queries.GetBacThueList
{
    public class GetBacThueListQueryHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly GetBacThueListQueryHandler _handler;

        public GetBacThueListQueryHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new GetBacThueListQueryHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_ReturnsSortedList()
        {
            _context.BacThues.AddRange(
                new BacThue { IdBacThue = Guid.NewGuid(), Bac = 2, TuGia = 5000000, DenGia = 10000000, ThueSuat = 10, IsActive = true },
                new BacThue { IdBacThue = Guid.NewGuid(), Bac = 1, TuGia = 0, DenGia = 5000000, ThueSuat = 5, IsActive = true }
            );
            await _context.SaveChangesAsync();

            var query = new GetBacThueListQuery();
            var result = await _handler.Handle(query, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            result.Data.Should().HaveCount(2);
            result.Data.First().Bac.Should().Be(1);
            result.Data.Last().Bac.Should().Be(2);
        }
    }
}
