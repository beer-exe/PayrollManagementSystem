using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Features.ThueTncn.Commands.UpsertCauHinhGiamTru;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;

namespace PayrollManagementSystem.UnitTests.Application.Features.ThueTncn.Commands.UpsertCauHinhGiamTru
{
    public class UpsertCauHinhGiamTruCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly UpsertCauHinhGiamTruCommandHandler _handler;

        public UpsertCauHinhGiamTruCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new UpsertCauHinhGiamTruCommandHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_NoExistingConfig_CreatesNew()
        {
            var command = new UpsertCauHinhGiamTruCommand
            {
                GiamTruBanThan = 11000000,
                GiamTruNguoiPhuThuoc = 4400000,
                GhiChu = "New config"
            };

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Succeeded.Should().BeTrue();

            var config = await _context.CauHinhGiamTrus.SingleOrDefaultAsync();
            config.Should().NotBeNull();
            config!.GiamTruBanThan.Should().Be(11000000);
            config.GiamTruNguoiPhuThuoc.Should().Be(4400000);
            config.GhiChu.Should().Be("New config");
        }

        [Fact]
        public async Task Handle_ExistingConfig_UpdatesIt()
        {
            var existingConfig = new CauHinhGiamTru
            {
                GiamTruBanThan = 11000000,
                GiamTruNguoiPhuThuoc = 4400000,
                GhiChu = "Old config",
                IsActive = true
            };
            _context.CauHinhGiamTrus.Add(existingConfig);
            await _context.SaveChangesAsync();

            var command = new UpsertCauHinhGiamTruCommand
            {
                GiamTruBanThan = 12000000,
                GiamTruNguoiPhuThuoc = 5000000,
                GhiChu = "Updated config"
            };

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Succeeded.Should().BeTrue();

            var count = await _context.CauHinhGiamTrus.CountAsync();
            count.Should().Be(1); // Should not create a new one

            var config = await _context.CauHinhGiamTrus.SingleAsync();
            config.GiamTruBanThan.Should().Be(12000000);
            config.GiamTruNguoiPhuThuoc.Should().Be(5000000);
            config.GhiChu.Should().Be("Updated config");
        }
    }
}
