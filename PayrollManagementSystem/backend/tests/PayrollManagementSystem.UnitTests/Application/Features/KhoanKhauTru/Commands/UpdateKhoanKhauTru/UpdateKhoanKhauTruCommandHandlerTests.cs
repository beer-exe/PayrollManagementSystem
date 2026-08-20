using FluentAssertions;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Features.KhoanKhauTru.Commands.UpdateKhoanKhauTru;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;

namespace PayrollManagementSystem.UnitTests.Application.Features.KhoanKhauTru.Commands.UpdateKhoanKhauTru
{
    public class UpdateKhoanKhauTruCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly UpdateKhoanKhauTruCommandHandler _handler;

        public UpdateKhoanKhauTruCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new UpdateKhoanKhauTruCommandHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_EntityNotFound_ThrowsApiException()
        {
            var command = new UpdateKhoanKhauTruCommand { IdKhoanKhauTru = Guid.NewGuid() };
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Không tìm thấy");
        }

        [Fact]
        public async Task Handle_DuplicateName_ThrowsApiException()
        {
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            _context.KhoanKhauTrus.Add(new Domain.Models.KhoanKhauTru { IdKhoanKhauTru = id1, TenKhoanKhauTru = "Bảo hiểm y tế" });
            _context.KhoanKhauTrus.Add(new Domain.Models.KhoanKhauTru { IdKhoanKhauTru = id2, TenKhoanKhauTru = "Bảo hiểm xã hội" });
            await _context.SaveChangesAsync();

            var command = new UpdateKhoanKhauTruCommand
            {
                IdKhoanKhauTru = id1,
                TenKhoanKhauTru = "Bảo hiểm xã hội"
            };

            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("đã tồn tại");
        }

        [Fact]
        public async Task Handle_ValidRequest_UpdatesEntity()
        {
            var id = Guid.NewGuid();
            _context.KhoanKhauTrus.Add(new Domain.Models.KhoanKhauTru { IdKhoanKhauTru = id, TenKhoanKhauTru = "Bảo hiểm y tế", IsActive = true });
            await _context.SaveChangesAsync();

            var command = new UpdateKhoanKhauTruCommand
            {
                IdKhoanKhauTru = id,
                TenKhoanKhauTru = "Bảo hiểm y tế cập nhật",
                LoaiCongThuc = LoaiCongThucKhauTru.SO_TIEN_CO_DINH,
                GiaTri = 500000,
                IsActive = false
            };

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Succeeded.Should().BeTrue();

            var entity = await _context.KhoanKhauTrus.FindAsync(id);
            entity!.TenKhoanKhauTru.Should().Be("Bảo hiểm y tế cập nhật");
            entity.LoaiCongThuc.Should().Be(LoaiCongThucKhauTru.SO_TIEN_CO_DINH);
            entity.GiaTri.Should().Be(500000);
            entity.IsActive.Should().BeFalse();
        }
    }
}
