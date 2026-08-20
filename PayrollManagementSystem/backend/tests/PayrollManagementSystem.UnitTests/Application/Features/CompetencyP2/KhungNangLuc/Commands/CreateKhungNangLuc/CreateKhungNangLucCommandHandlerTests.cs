using FluentAssertions;
using PayrollManagementSystem.Application.Features.CompetencyP2.KhungNangLuc.Commands.CreateKhungNangLuc;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;

namespace PayrollManagementSystem.UnitTests.Application.Features.CompetencyP2.KhungNangLuc.Commands.CreateKhungNangLuc
{
    public class CreateKhungNangLucCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly CreateKhungNangLucCommandHandler _handler;

        public CreateKhungNangLucCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new CreateKhungNangLucCommandHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_ValidCommand_AddsEntityAndReturnsId()
        {
            // Arrange
            var command = new CreateKhungNangLucCommand
            {
                IdChucVu = "CV001",
                TenNangLuc = "Giao tiếp",
                MoTa = "Kỹ năng giao tiếp tốt",
                TyTrong = 0.2m
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Succeeded.Should().BeTrue();
            result.Data.Should().NotBeEmpty();

            var entityInDb = await _context.KhungNangLucP2s.FindAsync(result.Data);
            entityInDb.Should().NotBeNull();
            entityInDb!.IdChucVu.Should().Be("CV001");
            entityInDb.TenNangLuc.Should().Be("Giao tiếp");
            entityInDb.TyTrong.Should().Be(0.2m);
        }
    }
}
