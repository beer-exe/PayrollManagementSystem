using FluentAssertions;
using PayrollManagementSystem.Application.Features.ThueTncn.Queries.GetCauHinhGiamTru;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;

namespace PayrollManagementSystem.UnitTests.Application.Features.ThueTncn.Queries.GetCauHinhGiamTru
{
    public class GetCauHinhGiamTruQueryHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly GetCauHinhGiamTruQueryHandler _handler;

        public GetCauHinhGiamTruQueryHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new GetCauHinhGiamTruQueryHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_NoConfig_ReturnsDefault()
        {
            var query = new GetCauHinhGiamTruQuery();
            var result = await _handler.Handle(query, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.GiamTruBanThan.Should().Be(11000000m);
            result.Data.GiamTruNguoiPhuThuoc.Should().Be(4400000m);
        }

        [Fact]
        public async Task Handle_HasConfig_ReturnsConfig()
        {
            _context.CauHinhGiamTrus.Add(new CauHinhGiamTru
            {
                IdCauHinhGiamTru = Guid.NewGuid(),
                GiamTruBanThan = 12000000m,
                GiamTruNguoiPhuThuoc = 5000000m,
                GhiChu = "Test config",
                IsActive = true
            });
            await _context.SaveChangesAsync();

            var query = new GetCauHinhGiamTruQuery();
            var result = await _handler.Handle(query, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.GiamTruBanThan.Should().Be(12000000m);
            result.Data.GiamTruNguoiPhuThuoc.Should().Be(5000000m);
        }
    }
}
