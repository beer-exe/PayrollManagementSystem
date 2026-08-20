using FluentAssertions;
using PayrollManagementSystem.Application.Features.Payroll.Queries.GetPayrollList;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;

namespace PayrollManagementSystem.UnitTests.Application.Features.Payroll.Queries.GetPayrollList
{
    public class GetPayrollListQueryHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly GetPayrollListQueryHandler _handler;

        public GetPayrollListQueryHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new GetPayrollListQueryHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_ReturnsPayrollList()
        {
            // Arrange
            var kyLuongId = Guid.NewGuid();
            var nhanVien = new NhanVien { Cccd = "001", HoTen = "Nguyen Van A" };
            _context.NhanViens.Add(nhanVien);

            var bangLuong = new BangLuong
            {
                IdBangLuong = Guid.NewGuid(),
                IdKyLuong = kyLuongId,
                CccdNhanVien = "001",
                Thang = 6,
                Nam = 2026,
                ChiTietKhauTru = "[]",
                ChiTietThue = "{}",
                P1 = 5000000,
                TongThuNhap = 6000000,
                ThucLinh = 5500000
            };
            _context.BangLuongs.Add(bangLuong);

            var phongBan = new PhongBan { IdPb = "PB01", TenPb = "Phòng IT" };
            var chucVu = new ChucVu { IdChucVu = "CV01", TenChucVu = "Developer", IdPhongBan = "PB01" };
            _context.PhongBans.Add(phongBan);
            _context.ChucVus.Add(chucVu);

            var quyetDinh = new QuyetDinhNhanSu
            {
                SoQuyetDinh = "QD-01",
                Cccd = "001",
                TrangThai = TrangThaiQuyetDinh.HIEU_LUC,
                LoaiQuyetDinh = "DIEU_CHUYEN",
                NgayHieuLuc = new DateOnly(2026, 1, 1),
                IdChucVuMoi = "CV01"
            };
            _context.QuyetDinhNhanSus.Add(quyetDinh);

            await _context.SaveChangesAsync();

            var query = new GetPayrollListQuery { Thang = 6, Nam = 2026 };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            result.Data.Should().HaveCount(1);

            var dto = result.Data.First();
            dto.CccdNhanVien.Should().Be("001");
            dto.TenNhanVien.Should().Be("Nguyen Van A");
            dto.TenPhongBan.Should().Be("Phòng IT");
            dto.TenChucVu.Should().Be("Developer");
            dto.ThucLinh.Should().Be(5500000);
        }

        [Fact]
        public async Task Handle_NoPayroll_ReturnsEmptyList()
        {
            var query = new GetPayrollListQuery { Thang = 6, Nam = 2026 };
            var result = await _handler.Handle(query, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            result.Data.Should().BeEmpty();
        }
    }
}
