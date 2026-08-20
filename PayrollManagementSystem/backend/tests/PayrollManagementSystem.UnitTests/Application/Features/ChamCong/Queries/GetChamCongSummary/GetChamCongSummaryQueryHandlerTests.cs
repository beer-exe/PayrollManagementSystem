using FluentAssertions;
using PayrollManagementSystem.Application.Features.ChamCong.Queries.GetChamCongSummary;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;

namespace PayrollManagementSystem.UnitTests.Application.Features.ChamCong.Queries.GetChamCongSummary
{
    public class GetChamCongSummaryQueryHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly GetChamCongSummaryQueryHandler _handler;

        public GetChamCongSummaryQueryHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new GetChamCongSummaryQueryHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsSummary()
        {
            // Arrange
            var nv = new NhanVien { Cccd = "001", HoTen = "Test NV", IdPb = "PB01", TrangThai = TrangThaiNhanVien.DANG_LAM_VIEC };

            _context.NhanViens.Add(nv);

            _context.ChamCongs.AddRange(new List<Domain.Models.ChamCong>
            {
                new Domain.Models.ChamCong { Id = Guid.NewGuid(), CccdNhanVien = "001", NgayChamCong = new DateOnly(2025, 1, 1), SoNgayCong = 1.0m, LoaiNgayCong = LoaiNgayCong.LAM_DU_CA },
                new Domain.Models.ChamCong { Id = Guid.NewGuid(), CccdNhanVien = "001", NgayChamCong = new DateOnly(2025, 1, 2), SoNgayCong = 0.5m, LoaiNgayCong = LoaiNgayCong.NUA_CA },
                new Domain.Models.ChamCong { Id = Guid.NewGuid(), CccdNhanVien = "001", NgayChamCong = new DateOnly(2025, 1, 3), SoNgayCong = 0, LoaiNgayCong = LoaiNgayCong.NGHI_LE }
            });
            await _context.SaveChangesAsync();

            var query = new GetChamCongSummaryQuery { Thang = 1, Nam = 2025 };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Should().HaveCount(1);

            var summary = result.Data!.First();
            summary.CccdNhanVien.Should().Be("001");
            summary.TongNgayCongThucTe.Should().Be(1.5m);
            summary.NgayNghiLe.Should().Be(1);
        }
    }
}
