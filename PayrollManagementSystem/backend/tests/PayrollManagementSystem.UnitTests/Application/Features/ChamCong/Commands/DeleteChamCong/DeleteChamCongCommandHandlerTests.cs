using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Features.ChamCong.Commands.DeleteChamCong;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.ChamCong.Commands.DeleteChamCong
{
    public class DeleteChamCongCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly DeleteChamCongCommandHandler _handler;

        public DeleteChamCongCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new DeleteChamCongCommandHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_ValidId_SoftDeletesEntity()
        {
            // Arrange
            var entity = new Domain.Models.ChamCong { Id = Guid.NewGuid(), CccdNhanVien = "001", NgayChamCong = new DateOnly(2025, 1, 1) };
            _context.ChamCongs.Add(entity);
            await _context.SaveChangesAsync();

            var command = new DeleteChamCongCommand { Id = entity.Id };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            
            var entityInDb = await _context.ChamCongs.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == entity.Id);
            entityInDb.Should().NotBeNull();
            entityInDb!.IsDeleted.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_InvalidId_ThrowsApiException()
        {
            // Arrange
            var command = new DeleteChamCongCommand { Id = Guid.NewGuid() };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Không tìm thấy bản ghi chấm công");
        }
    }
}
