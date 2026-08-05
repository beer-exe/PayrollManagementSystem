using FluentAssertions;
using PayrollManagementSystem.Application.Features.Users.Queries.GetRoles;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.Users.Queries.GetRoles
{
    public class GetRolesQueryHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly GetRolesQueryHandler _handler;

        public GetRolesQueryHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new GetRolesQueryHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_ReturnsRoles()
        {
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            _context.VaiTros.AddRange(
                new VaiTro { IdVaiTro = id1, TenVaiTro = "Admin" },
                new VaiTro { IdVaiTro = id2, TenVaiTro = "User" }
            );
            await _context.SaveChangesAsync();

            var query = new GetRolesQuery();
            var result = await _handler.Handle(query, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            result.Data.Should().HaveCount(2);
            result.Data.Should().Contain(x => x.IdVaiTro == id1 && x.TenVaiTro == "Admin");
            result.Data.Should().Contain(x => x.IdVaiTro == id2 && x.TenVaiTro == "User");
        }
    }
}
