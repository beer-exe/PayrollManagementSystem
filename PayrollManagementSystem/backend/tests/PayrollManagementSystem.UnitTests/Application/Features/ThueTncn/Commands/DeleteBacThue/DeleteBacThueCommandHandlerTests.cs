using FluentAssertions;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Features.ThueTncn.Commands.DeleteBacThue;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;

namespace PayrollManagementSystem.UnitTests.Application.Features.ThueTncn.Commands.DeleteBacThue
{
    public class DeleteBacThueCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly DeleteBacThueCommandHandler _handler;

        public DeleteBacThueCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new DeleteBacThueCommandHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_NotFound_ThrowsApiException()
        {
            var command = new DeleteBacThueCommand { IdBacThue = Guid.NewGuid() };

            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Không tìm thấy");
        }

        [Fact]
        public async Task Handle_ValidRequest_SoftDeletesBacThue()
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

            var command = new DeleteBacThueCommand { IdBacThue = id };

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            entity.IsDeleted.Should().BeTrue();
        }
    }
}
