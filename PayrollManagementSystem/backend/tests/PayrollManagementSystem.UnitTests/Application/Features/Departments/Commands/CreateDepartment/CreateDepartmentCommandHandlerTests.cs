using FluentAssertions;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Features.Departments.Commands.CreateDepartment;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.Departments.Commands.CreateDepartment
{
    public class CreateDepartmentCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly CreateDepartmentCommandHandler _handler;

        public CreateDepartmentCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new CreateDepartmentCommandHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_ValidRequest_CreatesDepartment()
        {
            // Arrange
            var command = new CreateDepartmentCommand { IdPb = "PB01", TenPb = "Phòng IT" };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            result.Data.Should().Be("PB01");

            var savedEntity = await _context.PhongBans.FindAsync("PB01");
            savedEntity.Should().NotBeNull();
            savedEntity!.TenPb.Should().Be("Phòng IT");
        }

        [Fact]
        public async Task Handle_DuplicateId_ThrowsApiException()
        {
            // Arrange
            _context.PhongBans.Add(new PhongBan { IdPb = "PB01", TenPb = "Phòng cũ" });
            await _context.SaveChangesAsync();

            var command = new CreateDepartmentCommand { IdPb = "PB01", TenPb = "Phòng IT" };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("đã tồn tại");
        }
    }
}
