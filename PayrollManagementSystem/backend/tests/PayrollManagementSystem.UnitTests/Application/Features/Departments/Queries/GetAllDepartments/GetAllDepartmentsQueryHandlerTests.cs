using FluentAssertions;
using PayrollManagementSystem.Application.Features.Departments.Queries.GetAllDepartments;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.Departments.Queries.GetAllDepartments
{
    public class GetAllDepartmentsQueryHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly GetAllDepartmentsQueryHandler _handler;

        public GetAllDepartmentsQueryHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new GetAllDepartmentsQueryHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_ReturnsAllDepartments()
        {
            // Arrange
            _context.PhongBans.AddRange(new List<PhongBan>
            {
                new PhongBan { IdPb = "PB01", TenPb = "Phòng IT" },
                new PhongBan { IdPb = "PB02", TenPb = "Phòng HR" }
            });
            await _context.SaveChangesAsync();

            var query = new GetAllDepartmentsQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            result.Data.Should().HaveCount(2);
            result.Data.Should().Contain(x => x.IdPb == "PB01");
            result.Data.Should().Contain(x => x.IdPb == "PB02");
        }
    }
}
