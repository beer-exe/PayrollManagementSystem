using FluentAssertions;
using PayrollManagementSystem.Application.Features.DonNghi.Queries.GetDonNghiList;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;

namespace PayrollManagementSystem.UnitTests.Application.Features.DonNghi.Queries.GetDonNghiList
{
    public class GetDonNghiListQueryHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly GetDonNghiListQueryHandler _handler;

        public GetDonNghiListQueryHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new GetDonNghiListQueryHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_ReturnsFilteredList()
        {
            // Arrange
            var pb = new PhongBan { IdPb = "PB1", TenPb = "Phòng 1" };
            var nv = new NhanVien { Cccd = "001", HoTen = "Test NV", IdPb = "PB1" };
            var donNghi = new Domain.Models.DonNghi
            {
                Id = Guid.NewGuid(),
                CccdNhanVien = "001",
                TrangThai = TrangThaiDonNghi.CHO_DUYET,
                LyDo = "Test",
                NgayBatDau = new DateOnly(2025, 5, 1),
                NgayKetThuc = new DateOnly(2025, 5, 2)
            };

            _context.PhongBans.Add(pb);
            _context.NhanViens.Add(nv);
            _context.DonNghis.Add(donNghi);
            await _context.SaveChangesAsync();

            var query = new GetDonNghiListQuery { Nam = 2025, Thang = 5, IdPhongBan = "PB1" };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            result.Data.Should().HaveCount(1);
            result.Data.First().HoTenNhanVien.Should().Be("Test NV");
            result.Data.First().TenPhongBan.Should().Be("Phòng 1");
        }
    }
}
