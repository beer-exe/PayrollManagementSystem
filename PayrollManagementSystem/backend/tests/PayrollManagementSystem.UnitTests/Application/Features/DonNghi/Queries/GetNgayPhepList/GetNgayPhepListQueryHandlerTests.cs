using FluentAssertions;
using PayrollManagementSystem.Application.Features.DonNghi.Queries.GetNgayPhepList;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;

namespace PayrollManagementSystem.UnitTests.Application.Features.DonNghi.Queries.GetNgayPhepList
{
    public class GetNgayPhepListQueryHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly GetNgayPhepListQueryHandler _handler;

        public GetNgayPhepListQueryHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new GetNgayPhepListQueryHandler(_context);
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
            var ngayPhep = new NgayPhepNhanVien { Id = Guid.NewGuid(), CccdNhanVien = "001", Nam = 2025, TongNgayPhep = 12, DaSuDung = 2 };

            _context.PhongBans.Add(pb);
            _context.NhanViens.Add(nv);
            _context.NgayPhepNhanViens.Add(ngayPhep);
            await _context.SaveChangesAsync();

            var query = new GetNgayPhepListQuery { Nam = 2025, IdPhongBan = "PB1" };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            result.Data.Should().HaveCount(1);
            result.Data.First().HoTenNhanVien.Should().Be("Test NV");
            result.Data.First().TongNgayPhep.Should().Be(12);
            result.Data.First().DaSuDung.Should().Be(2);
            result.Data.First().ConLai.Should().Be(10);
        }
    }
}
