using FluentAssertions;
using PayrollManagementSystem.Application.Features.CompetencyP2.KhungNangLuc.Queries.GetKhungNangLucs;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.CompetencyP2.KhungNangLuc.Queries.GetKhungNangLucs
{
    public class GetKhungNangLucsQueryHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly GetKhungNangLucsQueryHandler _handler;

        public GetKhungNangLucsQueryHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new GetKhungNangLucsQueryHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsList()
        {
            // Arrange
            _context.KhungNangLucP2s.AddRange(new List<KhungNangLucP2>
            {
                new KhungNangLucP2 { IdTieuChi = Guid.NewGuid(), IdChucVu = "CV01", TenNangLuc = "Test 1", TyTrong = 0.5m },
                new KhungNangLucP2 { IdTieuChi = Guid.NewGuid(), IdChucVu = "CV01", TenNangLuc = "Test 2", TyTrong = 0.5m },
                new KhungNangLucP2 { IdTieuChi = Guid.NewGuid(), IdChucVu = "CV02", TenNangLuc = "Test 3", TyTrong = 1.0m }
            });
            await _context.SaveChangesAsync();

            var query = new GetKhungNangLucsQuery { IdChucVu = "CV01" };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Should().HaveCount(2);
            result.Data!.All(x => x.IdChucVu == "CV01").Should().BeTrue();
        }

        [Fact]
        public async Task Handle_NoRecords_ReturnsEmptyList()
        {
            // Arrange
            var query = new GetKhungNangLucsQuery { IdChucVu = "CV03" };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Should().BeEmpty();
        }
    }
}
