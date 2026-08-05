using FluentAssertions;
using PayrollManagementSystem.Application.Features.ChamCong.Queries.GetChamCongByNhanVien;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.ChamCong.Queries.GetChamCongByNhanVien
{
    public class GetChamCongByNhanVienQueryHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly GetChamCongByNhanVienQueryHandler _handler;

        public GetChamCongByNhanVienQueryHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new GetChamCongByNhanVienQueryHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsList()
        {
            // Arrange
            var nv = new NhanVien { Cccd = "001", HoTen = "Test NV", IdPb = "PB01" };
            var nv2 = new NhanVien { Cccd = "002", HoTen = "Test NV2", IdPb = "PB02" };
            
            _context.NhanViens.AddRange(nv, nv2);

            _context.ChamCongs.AddRange(new List<Domain.Models.ChamCong>
            {
                new Domain.Models.ChamCong { Id = Guid.NewGuid(), CccdNhanVien = "001", NgayChamCong = new DateOnly(2025, 1, 1), TrangThai = TrangThaiChamCong.DA_XAC_NHAN, NhanVien = nv },
                new Domain.Models.ChamCong { Id = Guid.NewGuid(), CccdNhanVien = "002", NgayChamCong = new DateOnly(2025, 1, 1), TrangThai = TrangThaiChamCong.DA_XAC_NHAN, NhanVien = nv2 },
                new Domain.Models.ChamCong { Id = Guid.NewGuid(), CccdNhanVien = "001", NgayChamCong = new DateOnly(2025, 2, 1), TrangThai = TrangThaiChamCong.DA_XAC_NHAN, NhanVien = nv }
            });
            await _context.SaveChangesAsync();

            var query = new GetChamCongByNhanVienQuery { Thang = 1, Nam = 2025, IdPhongBan = "PB01" };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Should().HaveCount(1);
            result.Data!.First().CccdNhanVien.Should().Be("001");
            result.Data!.First().NgayChamCong.Should().Be("2025-01-01");
        }
    }
}
