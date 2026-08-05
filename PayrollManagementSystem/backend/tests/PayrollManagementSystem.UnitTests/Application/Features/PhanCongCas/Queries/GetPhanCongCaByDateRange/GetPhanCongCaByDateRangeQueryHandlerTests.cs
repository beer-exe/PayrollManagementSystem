using FluentAssertions;
using PayrollManagementSystem.Application.Features.PhanCongCas.Queries.GetPhanCongCaByDateRange;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.PhanCongCas.Queries.GetPhanCongCaByDateRange
{
    public class GetPhanCongCaByDateRangeQueryHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly GetPhanCongCaByDateRangeQueryHandler _handler;

        public GetPhanCongCaByDateRangeQueryHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new GetPhanCongCaByDateRangeQueryHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_ReturnsPhanCongCaWithinDateRange()
        {
            var pb = new PhongBan { IdPb = "PB01", TenPb = "IT" };
            _context.PhongBans.Add(pb);

            var nv1 = new NhanVien { Cccd = "001", HoTen = "NV1", IdPb = "PB01", PhongBan = pb };
            var nv2 = new NhanVien { Cccd = "002", HoTen = "NV2", IdPb = "PB01", PhongBan = pb };
            _context.NhanViens.AddRange(nv1, nv2);

            var caId = Guid.NewGuid();
            var ca = new CaLamViec { Id = caId, TenCa = "Ca 1", GioBatDau = new TimeSpan(8,0,0), GioKetThuc = new TimeSpan(17,0,0) };
            _context.CaLamViecs.Add(ca);

            var date1 = DateOnly.FromDateTime(DateTime.Today);
            var date2 = date1.AddDays(1);
            var date3 = date1.AddDays(5); // outside range

            _context.PhanCongCas.AddRange(
                new PhanCongCa { CccdNhanVien = "001", NgayLamViec = date1, IdCaLamViec = caId, NhanVien = nv1, CaLamViec = ca, IsDeleted = false },
                new PhanCongCa { CccdNhanVien = "002", NgayLamViec = date2, IdCaLamViec = null, NhanVien = nv2, IsDeleted = false },
                new PhanCongCa { CccdNhanVien = "001", NgayLamViec = date3, IdCaLamViec = caId, NhanVien = nv1, CaLamViec = ca, IsDeleted = false }, // Should not be returned
                new PhanCongCa { CccdNhanVien = "001", NgayLamViec = date1, IdCaLamViec = caId, NhanVien = nv1, CaLamViec = ca, IsDeleted = true } // Should not be returned (deleted)
            );
            await _context.SaveChangesAsync();

            var query = new GetPhanCongCaByDateRangeQuery
            {
                StartDate = date1,
                EndDate = date2
            };

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            result.Data.Should().HaveCount(2);
            result.Data.Should().Contain(p => p.CccdNhanVien == "001" && p.TenCa == "Ca 1");
            result.Data.Should().Contain(p => p.CccdNhanVien == "002" && p.IdCaLamViec == null);
        }

        [Fact]
        public async Task Handle_WithIdPhongBan_ReturnsFilteredPhanCongCa()
        {
            var pb1 = new PhongBan { IdPb = "PB01", TenPb = "IT" };
            var pb2 = new PhongBan { IdPb = "PB02", TenPb = "HR" };
            _context.PhongBans.AddRange(pb1, pb2);

            var nv1 = new NhanVien { Cccd = "001", HoTen = "NV1", IdPb = "PB01", PhongBan = pb1 };
            var nv2 = new NhanVien { Cccd = "002", HoTen = "NV2", IdPb = "PB02", PhongBan = pb2 };
            _context.NhanViens.AddRange(nv1, nv2);

            var date = DateOnly.FromDateTime(DateTime.Today);

            _context.PhanCongCas.AddRange(
                new PhanCongCa { CccdNhanVien = "001", NgayLamViec = date, IdCaLamViec = null, NhanVien = nv1 },
                new PhanCongCa { CccdNhanVien = "002", NgayLamViec = date, IdCaLamViec = null, NhanVien = nv2 }
            );
            await _context.SaveChangesAsync();

            var query = new GetPhanCongCaByDateRangeQuery
            {
                StartDate = date,
                EndDate = date,
                IdPhongBan = "PB01"
            };

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            result.Data.Should().HaveCount(1);
            result.Data.First().CccdNhanVien.Should().Be("001");
        }
    }
}
