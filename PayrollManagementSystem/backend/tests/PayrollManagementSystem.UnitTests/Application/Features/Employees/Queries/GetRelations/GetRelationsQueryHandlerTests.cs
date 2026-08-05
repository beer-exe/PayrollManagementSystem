using FluentAssertions;
using PayrollManagementSystem.Application.Features.Employees.Queries.GetRelations;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.Employees.Queries.GetRelations
{
    public class GetRelationsQueryHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly GetRelationsQueryHandler _handler;

        public GetRelationsQueryHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new GetRelationsQueryHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_ReturnsAllRelations()
        {
            // Arrange
            _context.MoiQuanHes.Add(new MoiQuanHe { IdMqh = Guid.NewGuid(), TenQuanHe = "Vợ/Chồng" });
            _context.MoiQuanHes.Add(new MoiQuanHe { IdMqh = Guid.NewGuid(), TenQuanHe = "Con" });
            await _context.SaveChangesAsync();

            var query = new GetRelationsQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            result.Data.Should().HaveCount(2);
            result.Data.Should().Contain(r => r.TenQuanHe == "Vợ/Chồng");
            result.Data.Should().Contain(r => r.TenQuanHe == "Con");
        }
    }
}
