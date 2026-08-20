using FluentAssertions;
using PayrollManagementSystem.Application.Features.CompetencyP2.KyDanhGia.Queries.GetKyDanhGias;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;

namespace PayrollManagementSystem.UnitTests.Application.Features.CompetencyP2.KyDanhGia.Queries.GetKyDanhGias
{
    public class GetKyDanhGiasQueryHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly GetKyDanhGiasQueryHandler _handler;

        public GetKyDanhGiasQueryHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new GetKyDanhGiasQueryHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsList()
        {
            // Arrange
            _context.KyDanhGias.AddRange(new List<Domain.Models.KyDanhGia>
            {
                new Domain.Models.KyDanhGia { IdKyDanhGia = Guid.NewGuid(), TenKyDanhGia = "Ky 1", Nam = 2025, TrangThai = TrangThaiKyDanhGia.KHOI_TAO },
                new Domain.Models.KyDanhGia { IdKyDanhGia = Guid.NewGuid(), TenKyDanhGia = "Ky 2", Nam = 2025, TrangThai = TrangThaiKyDanhGia.DANG_DANH_GIA }
            });
            await _context.SaveChangesAsync();

            var query = new GetKyDanhGiasQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Should().HaveCount(2);
        }
    }
}
