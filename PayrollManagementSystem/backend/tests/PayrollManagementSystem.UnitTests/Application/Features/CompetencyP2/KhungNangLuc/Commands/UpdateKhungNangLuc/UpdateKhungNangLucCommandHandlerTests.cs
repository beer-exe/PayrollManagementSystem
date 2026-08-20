using FluentAssertions;
using PayrollManagementSystem.Application.Features.CompetencyP2.KhungNangLuc.Commands.UpdateKhungNangLuc;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;

namespace PayrollManagementSystem.UnitTests.Application.Features.CompetencyP2.KhungNangLuc.Commands.UpdateKhungNangLuc
{
    public class UpdateKhungNangLucCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly UpdateKhungNangLucCommandHandler _handler;

        public UpdateKhungNangLucCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new UpdateKhungNangLucCommandHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_ValidCommand_UpdatesEntity()
        {
            // Arrange
            var entity = new KhungNangLucP2 { IdTieuChi = Guid.NewGuid(), IdChucVu = "CV01", TenNangLuc = "Test", TyTrong = 0.5m };
            _context.KhungNangLucP2s.Add(entity);
            await _context.SaveChangesAsync();

            var command = new UpdateKhungNangLucCommand
            {
                IdTieuChi = entity.IdTieuChi,
                TenNangLuc = "Updated",
                MoTa = "Updated Desc",
                TyTrong = 0.8m
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            result.Data.Should().BeTrue();

            var entityInDb = await _context.KhungNangLucP2s.FindAsync(entity.IdTieuChi);
            entityInDb.Should().NotBeNull();
            entityInDb!.TenNangLuc.Should().Be("Updated");
            entityInDb.MoTa.Should().Be("Updated Desc");
            entityInDb.TyTrong.Should().Be(0.8m);
        }

        [Fact]
        public async Task Handle_InvalidId_ReturnsErrorResponse()
        {
            // Arrange
            var command = new UpdateKhungNangLucCommand { IdTieuChi = Guid.NewGuid() };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Message.Should().Be("Không tìm thấy tiêu chí.");
        }
    }
}
