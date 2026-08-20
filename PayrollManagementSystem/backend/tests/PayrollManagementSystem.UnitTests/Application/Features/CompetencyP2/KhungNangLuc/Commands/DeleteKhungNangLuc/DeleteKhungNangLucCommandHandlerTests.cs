using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Features.CompetencyP2.KhungNangLuc.Commands.DeleteKhungNangLuc;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;

namespace PayrollManagementSystem.UnitTests.Application.Features.CompetencyP2.KhungNangLuc.Commands.DeleteKhungNangLuc
{
    public class DeleteKhungNangLucCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly DeleteKhungNangLucCommandHandler _handler;

        public DeleteKhungNangLucCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new DeleteKhungNangLucCommandHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_ValidId_DeletesEntity()
        {
            // Arrange
            var entity = new KhungNangLucP2 { IdTieuChi = Guid.NewGuid(), IdChucVu = "CV01", TenNangLuc = "Test", TyTrong = 0.5m };
            _context.KhungNangLucP2s.Add(entity);
            await _context.SaveChangesAsync();

            var command = new DeleteKhungNangLucCommand { IdTieuChi = entity.IdTieuChi };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            result.Data.Should().BeTrue();

            var entityInDb = await _context.KhungNangLucP2s.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.IdTieuChi == entity.IdTieuChi);
            entityInDb.Should().NotBeNull();
            entityInDb!.IsDeleted.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_InvalidId_ReturnsErrorResponse()
        {
            // Arrange
            var command = new DeleteKhungNangLucCommand { IdTieuChi = Guid.NewGuid() };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Message.Should().Be("Không tìm thấy tiêu chí.");
        }
    }
}
