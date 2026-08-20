using FluentAssertions;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Features.ThueTncn.Commands.UpdateBacThue;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;

namespace PayrollManagementSystem.UnitTests.Application.Features.ThueTncn.Commands.UpdateBacThue
{
    public class UpdateBacThueCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly UpdateBacThueCommandHandler _handler;

        public UpdateBacThueCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new UpdateBacThueCommandHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_NotFound_ThrowsApiException()
        {
            var command = new UpdateBacThueCommand { IdBacThue = Guid.NewGuid() };

            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Không tìm thấy");
        }

        [Fact]
        public async Task Handle_ValidRequest_UpdatesBacThue()
        {
            var id = Guid.NewGuid();
            var entity = new BacThue
            {
                IdBacThue = id,
                Bac = 1,
                TuGia = 0,
                DenGia = 5000000,
                ThueSuat = 5,
                IsActive = true
            };
            _context.BacThues.Add(entity);
            await _context.SaveChangesAsync();

            var command = new UpdateBacThueCommand
            {
                IdBacThue = id,
                TuGia = 0,
                DenGia = 10000000,
                ThueSuat = 10,
                IsActive = false
            };

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Succeeded.Should().BeTrue();

            var updated = await _context.BacThues.FindAsync(id);
            updated!.DenGia.Should().Be(10000000);
            updated.ThueSuat.Should().Be(10);
            updated.IsActive.Should().BeFalse();
        }
    }
}
