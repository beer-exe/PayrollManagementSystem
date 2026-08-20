using FluentAssertions;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Features.Positions.Commands.CreatePosition;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;

namespace PayrollManagementSystem.UnitTests.Application.Features.Positions.Commands.CreatePosition
{
    public class CreatePositionCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly CreatePositionCommandHandler _handler;

        public CreatePositionCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new CreatePositionCommandHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_DuplicatedId_ThrowsApiException()
        {
            // Arrange
            _context.ChucVus.Add(new ChucVu { IdChucVu = "CV01", TenChucVu = "Manager", IdPhongBan = "PB01" });
            await _context.SaveChangesAsync();

            var command = new CreatePositionCommand { IdChucVu = "CV01", TenChucVu = "Dev" };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("đã tồn tại");
        }

        [Fact]
        public async Task Handle_ValidRequest_CreatesPosition()
        {
            // Arrange
            var command = new CreatePositionCommand
            {
                IdChucVu = "CV02",
                TenChucVu = "Dev",
                MoTaCongViec = "Lập trình viên",
                IdPhongBan = "PB01"
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            result.Data.Should().Be("CV02");

            var entity = await _context.ChucVus.FindAsync("CV02");
            entity.Should().NotBeNull();
            entity!.TenChucVu.Should().Be("Dev");
        }
    }
}
