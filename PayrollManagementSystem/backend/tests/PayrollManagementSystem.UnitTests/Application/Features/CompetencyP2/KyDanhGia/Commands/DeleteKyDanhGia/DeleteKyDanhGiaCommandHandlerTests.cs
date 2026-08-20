using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Features.CompetencyP2.KyDanhGia.Commands.DeleteKyDanhGia;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;

namespace PayrollManagementSystem.UnitTests.Application.Features.CompetencyP2.KyDanhGia.Commands.DeleteKyDanhGia
{
    public class DeleteKyDanhGiaCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly DeleteKyDanhGiaCommandHandler _handler;

        public DeleteKyDanhGiaCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new DeleteKyDanhGiaCommandHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_ValidId_DeletesEntity()
        {
            // Arrange
            var entity = new Domain.Models.KyDanhGia { IdKyDanhGia = Guid.NewGuid(), TenKyDanhGia = "Test", TrangThai = TrangThaiKyDanhGia.KHOI_TAO };
            _context.KyDanhGias.Add(entity);
            await _context.SaveChangesAsync();

            var command = new DeleteKyDanhGiaCommand { IdKyDanhGia = entity.IdKyDanhGia };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            result.Data.Should().BeTrue();

            var entityInDb = await _context.KyDanhGias.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.IdKyDanhGia == entity.IdKyDanhGia);
            entityInDb.Should().NotBeNull();
            entityInDb!.IsDeleted.Should().BeTrue();
        }
    }
}
