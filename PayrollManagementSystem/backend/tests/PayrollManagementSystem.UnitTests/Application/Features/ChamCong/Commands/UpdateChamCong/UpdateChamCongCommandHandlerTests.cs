using FluentAssertions;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Features.ChamCong.Commands.UpdateChamCong;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.ChamCong.Commands.UpdateChamCong
{
    public class UpdateChamCongCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly UpdateChamCongCommandHandler _handler;

        public UpdateChamCongCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new UpdateChamCongCommandHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_ValidCommand_UpdatesEntity()
        {
            // Arrange
            var entity = new Domain.Models.ChamCong 
            { 
                Id = Guid.NewGuid(), 
                CccdNhanVien = "001", 
                NgayChamCong = new DateOnly(2025, 1, 1),
                GioVao = new TimeOnly(8, 0, 0),
                GioRa = new TimeOnly(17, 0, 0)
            };
            
            _context.ChamCongs.Add(entity);
            await _context.SaveChangesAsync();

            var command = new UpdateChamCongCommand 
            { 
                Id = entity.Id, 
                GioVao = new TimeOnly(9, 0, 0), // Late
                GioRa = new TimeOnly(17, 0, 0),
                GhiChu = "Updated note"
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            
            var updatedEntity = await _context.ChamCongs.FindAsync(entity.Id);
            updatedEntity.Should().NotBeNull();
            updatedEntity!.GioVao.Should().Be(new TimeOnly(9, 0, 0));
            updatedEntity.GhiChu.Should().Be("Updated note");
            // Since 9:00 to 17:00 is 8 hours total, minus 1 lunch = 7 hours work
            // 7 / 8 = 0.875 -> LoaiNgayCong should be DI_TRE_VE_SOM
            updatedEntity.LoaiNgayCong.Should().Be(LoaiNgayCong.DI_TRE_VE_SOM);
            updatedEntity.SoNgayCong.Should().Be(0.88m); // Math.Round(0.875, 2)
        }
    }
}
