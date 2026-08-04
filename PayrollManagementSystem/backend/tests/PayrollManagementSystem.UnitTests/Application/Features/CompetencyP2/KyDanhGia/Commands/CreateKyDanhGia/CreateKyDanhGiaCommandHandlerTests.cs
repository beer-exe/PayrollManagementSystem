using FluentAssertions;
using PayrollManagementSystem.Application.Features.CompetencyP2.KyDanhGia.Commands.CreateKyDanhGia;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.CompetencyP2.KyDanhGia.Commands.CreateKyDanhGia
{
    public class CreateKyDanhGiaCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly CreateKyDanhGiaCommandHandler _handler;

        public CreateKyDanhGiaCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new CreateKyDanhGiaCommandHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_ValidCommand_AddsEntityAndReturnsId()
        {
            // Arrange
            var startDate = new DateOnly(2025, 1, 1);
            var endDate = new DateOnly(2025, 12, 31);
            var command = new CreateKyDanhGiaCommand
            {
                TenKyDanhGia = "Kỳ đánh giá 2025",
                NgayBatDau = startDate,
                NgayKetThuc = endDate
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Succeeded.Should().BeTrue();
            result.Data.Should().NotBeEmpty();

            var entityInDb = await _context.KyDanhGias.FindAsync(result.Data);
            entityInDb.Should().NotBeNull();
            entityInDb!.TenKyDanhGia.Should().Be("Kỳ đánh giá 2025");
            entityInDb.Nam.Should().Be(2025);
            entityInDb.NgayBatDau.Should().Be(startDate);
            entityInDb.NgayKetThuc.Should().Be(endDate);
            entityInDb.TrangThai.Should().Be(TrangThaiKyDanhGia.KHOI_TAO);
        }
    }
}
