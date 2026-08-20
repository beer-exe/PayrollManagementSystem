using FluentAssertions;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Features.KhoanKhauTru.Commands.DeleteKhoanKhauTru;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;

namespace PayrollManagementSystem.UnitTests.Application.Features.KhoanKhauTru.Commands.DeleteKhoanKhauTru
{
    public class DeleteKhoanKhauTruCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly DeleteKhoanKhauTruCommandHandler _handler;

        public DeleteKhoanKhauTruCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new DeleteKhoanKhauTruCommandHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_EntityNotFound_ThrowsApiException()
        {
            var command = new DeleteKhoanKhauTruCommand { IdKhoanKhauTru = Guid.NewGuid() };
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Không tìm thấy");
        }

        [Fact]
        public async Task Handle_ValidRequest_SoftDeletesEntity()
        {
            var id = Guid.NewGuid();
            _context.KhoanKhauTrus.Add(new Domain.Models.KhoanKhauTru { IdKhoanKhauTru = id, TenKhoanKhauTru = "Bảo hiểm" });
            await _context.SaveChangesAsync();

            var command = new DeleteKhoanKhauTruCommand { IdKhoanKhauTru = id };
            var result = await _handler.Handle(command, CancellationToken.None);

            result.Succeeded.Should().BeTrue();

            var entity = await _context.KhoanKhauTrus.FindAsync(id);
            entity.Should().NotBeNull();
            entity!.IsDeleted.Should().BeTrue();
        }
    }
}
