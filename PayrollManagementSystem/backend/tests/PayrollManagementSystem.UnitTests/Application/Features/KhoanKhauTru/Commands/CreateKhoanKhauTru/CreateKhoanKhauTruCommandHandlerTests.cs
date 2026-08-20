using FluentAssertions;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Features.KhoanKhauTru.Commands.CreateKhoanKhauTru;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;

namespace PayrollManagementSystem.UnitTests.Application.Features.KhoanKhauTru.Commands.CreateKhoanKhauTru
{
    public class CreateKhoanKhauTruCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly CreateKhoanKhauTruCommandHandler _handler;

        public CreateKhoanKhauTruCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new CreateKhoanKhauTruCommandHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_DuplicateName_ThrowsApiException()
        {
            _context.KhoanKhauTrus.Add(new Domain.Models.KhoanKhauTru { TenKhoanKhauTru = "Bảo hiểm xã hội" });
            await _context.SaveChangesAsync();

            var command = new CreateKhoanKhauTruCommand
            {
                TenKhoanKhauTru = "Bảo hiểm xã hội"
            };

            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("đã tồn tại");
        }

        [Fact]
        public async Task Handle_ValidRequest_CreatesEntity()
        {
            var command = new CreateKhoanKhauTruCommand
            {
                TenKhoanKhauTru = "Thuế TNCN",
                LoaiCongThuc = LoaiCongThucKhauTru.TY_LE_PHAN_TRAM,
                GiaTri = 10,
                IsActive = true
            };

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            result.Message.Should().Contain("thành công");

            var entity = await _context.KhoanKhauTrus.FindAsync(result.Data);
            entity.Should().NotBeNull();
            entity!.TenKhoanKhauTru.Should().Be("Thuế TNCN");
            entity.LoaiCongThuc.Should().Be(LoaiCongThucKhauTru.TY_LE_PHAN_TRAM);
            entity.GiaTri.Should().Be(10);
            entity.IsActive.Should().BeTrue();
        }
    }
}
