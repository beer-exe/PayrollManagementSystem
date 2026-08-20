using FluentAssertions;
using PayrollManagementSystem.Application.Features.WorkSchedule.Queries.GetLichLamViecs;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;

namespace PayrollManagementSystem.UnitTests.Application.Features.WorkSchedule.Queries.GetLichLamViecs
{
    public class GetLichLamViecsQueryHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly GetLichLamViecsQueryHandler _handler;

        public GetLichLamViecsQueryHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new GetLichLamViecsQueryHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_ReturnsSchedulesWithComputedDays()
        {
            var lichId1 = Guid.NewGuid();
            var lich1 = new LichLamViec
            {
                IdLich = lichId1,
                Nam = 2024,
                TrangThai = TrangThaiLichLamViec.HIEU_LUC,
                ChiTietLichLamViecs = new List<ChiTietLichLamViec>
                {
                    new ChiTietLichLamViec { Id = Guid.NewGuid(), IdLich = lichId1, Ngay = new DateOnly(2024,1,1), Thu = "Hai", LoaiNgay = LoaiNgay.NGAY_LAM_VIEC, IsDeleted = false },
                    new ChiTietLichLamViec { Id = Guid.NewGuid(), IdLich = lichId1, Ngay = new DateOnly(2024,1,2), Thu = "Ba", LoaiNgay = LoaiNgay.NGHI_CUOI_TUAN, IsDeleted = false },
                    new ChiTietLichLamViec { Id = Guid.NewGuid(), IdLich = lichId1, Ngay = new DateOnly(2024,1,3), Thu = "Tư", LoaiNgay = LoaiNgay.NGHI_LE, IsDeleted = false },
                    new ChiTietLichLamViec { Id = Guid.NewGuid(), IdLich = lichId1, Ngay = new DateOnly(2024,1,4), Thu = "Năm", LoaiNgay = LoaiNgay.NGAY_LAM_VIEC, IsDeleted = true } // Should not count
                }
            };

            var lichId2 = Guid.NewGuid();
            var lich2 = new LichLamViec
            {
                IdLich = lichId2,
                Nam = 2023,
                TrangThai = TrangThaiLichLamViec.HET_HIEU_LUC,
                ChiTietLichLamViecs = new List<ChiTietLichLamViec>()
            };

            _context.LichLamViecs.AddRange(lich1, lich2);
            await _context.SaveChangesAsync();

            var query = new GetLichLamViecsQuery();
            var result = await _handler.Handle(query, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            result.Data.Should().HaveCount(2);

            var dto1 = result.Data.First(d => d.IdLich == lichId1);
            dto1.Nam.Should().Be(2024);
            dto1.TongNgay.Should().Be(3);
            dto1.TongNgayLam.Should().Be(1);
            dto1.TongNgayNghiCuoiTuan.Should().Be(1);
            dto1.TongNgayLe.Should().Be(1);
        }
    }
}
