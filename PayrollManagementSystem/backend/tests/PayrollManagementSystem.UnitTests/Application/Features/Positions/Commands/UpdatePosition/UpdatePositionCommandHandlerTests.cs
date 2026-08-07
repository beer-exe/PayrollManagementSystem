using FluentAssertions;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Features.Positions.Commands.UpdatePosition;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.Positions.Commands.UpdatePosition
{
    public class UpdatePositionCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly UpdatePositionCommandHandler _handler;

        public UpdatePositionCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new UpdatePositionCommandHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_NotFound_ThrowsApiException()
        {
            var command = new UpdatePositionCommand { IdChucVu = "NON_EXIST", TenChucVu = "Test" };
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("không tồn tại");
        }

        [Fact]
        public async Task Handle_ValidRequest_UpdatesPosition()
        {
            _context.ChucVus.Add(new ChucVu { IdChucVu = "CV01", TenChucVu = "Old Name", IdPhongBan = "PB01" });
            await _context.SaveChangesAsync();

            var command = new UpdatePositionCommand 
            { 
                IdChucVu = "CV01", 
                TenChucVu = "New Name",
                IdPhongBan = "PB01"
            };

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            
            var entity = await _context.ChucVus.FindAsync("CV01");
            entity!.TenChucVu.Should().Be("New Name");
            entity.IdPhongBan.Should().Be("PB01");
        }
    }
}
