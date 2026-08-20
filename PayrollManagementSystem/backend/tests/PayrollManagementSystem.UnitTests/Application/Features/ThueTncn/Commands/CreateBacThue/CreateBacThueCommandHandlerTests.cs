using FluentAssertions;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Features.ThueTncn.Commands.CreateBacThue;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;

namespace PayrollManagementSystem.UnitTests.Application.Features.ThueTncn.Commands.CreateBacThue
{
    public class CreateBacThueCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly CreateBacThueCommandHandler _handler;

        public CreateBacThueCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new CreateBacThueCommandHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_BacExists_ThrowsApiException()
        {
            _context.BacThues.Add(new BacThue
            {
                Bac = 1,
                TuGia = 0,
                DenGia = 5000000,
                ThueSuat = 5,
                IsActive = true
            });
            await _context.SaveChangesAsync();

            var command = new CreateBacThueCommand
            {
                Bac = 1,
                TuGia = 0,
                DenGia = 5000000,
                ThueSuat = 5
            };

            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("đã tồn tại");
        }

        [Fact]
        public async Task Handle_ValidRequest_CreatesBacThue()
        {
            var command = new CreateBacThueCommand
            {
                Bac = 1,
                TuGia = 0,
                DenGia = 5000000,
                ThueSuat = 5
            };

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            result.Data.Should().NotBeEmpty();

            var saved = await _context.BacThues.FindAsync(result.Data);
            saved.Should().NotBeNull();
            saved!.Bac.Should().Be(1);
            saved.TuGia.Should().Be(0);
            saved.DenGia.Should().Be(5000000);
            saved.ThueSuat.Should().Be(5);
            saved.IsActive.Should().BeTrue();
        }
    }
}
