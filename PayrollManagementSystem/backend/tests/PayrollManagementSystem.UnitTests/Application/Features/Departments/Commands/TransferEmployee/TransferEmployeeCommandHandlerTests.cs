using FluentAssertions;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Features.Departments.Commands.TransferEmployee;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.Departments.Commands.TransferEmployee
{
    public class TransferEmployeeCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly TransferEmployeeCommandHandler _handler;

        public TransferEmployeeCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new TransferEmployeeCommandHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_EmployeeNotFound_ThrowsApiException()
        {
            var command = new TransferEmployeeCommand { Cccd = "001", SoQuyetDinh = "QD01", IdPbMoi = "PB1", IdChucVuMoi = "CV1", IdBacLuongMoi = "B1", NgayHieuLuc = DateOnly.FromDateTime(DateTime.Now) };

            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Không tìm thấy nhân viên");
        }

        [Fact]
        public async Task Handle_InvalidDepartment_ThrowsApiException()
        {
            _context.NhanViens.Add(new NhanVien { Cccd = "001", HoTen = "Test NV" });
            await _context.SaveChangesAsync();

            var command = new TransferEmployeeCommand { Cccd = "001", SoQuyetDinh = "QD01", IdPbMoi = "INVALID", IdChucVuMoi = "CV1", IdBacLuongMoi = "B1", NgayHieuLuc = DateOnly.FromDateTime(DateTime.Now) };

            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Phòng ban mới với mã 'INVALID' không tồn tại");
        }

        [Fact]
        public async Task Handle_ValidRequest_UpdatesDepartmentAndCreatesQuyetDinh()
        {
            // Arrange
            var nv = new NhanVien { Cccd = "001", HoTen = "Test NV", IdPb = "PB_OLD" };
            var pbNew = new PhongBan { IdPb = "PB_NEW", TenPb = "Phòng Mới" };
            var cvMoi = new ChucVu { IdChucVu = "CV1", TenChucVu = "Chức vụ 1", IdNgachLuong = "NL1", IdPhongBan = "PB_NEW" };
            var bacLuong = new BacLuong { IdBacLuong = "B1", TenBacLuong = "Bậc 1", IdNgachLuong = "NL1" };

            _context.NhanViens.Add(nv);
            _context.PhongBans.Add(pbNew);
            _context.ChucVus.Add(cvMoi);
            _context.BacLuongs.Add(bacLuong);
            await _context.SaveChangesAsync();

            var command = new TransferEmployeeCommand 
            { 
                Cccd = "001", 
                SoQuyetDinh = "QD01", 
                IdPbMoi = "PB_NEW", 
                IdChucVuMoi = "CV1", 
                IdBacLuongMoi = "B1", 
                NgayHieuLuc = DateOnly.FromDateTime(DateTime.Today) 
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();

            var dbNv = await _context.NhanViens.FindAsync("001");
            dbNv!.IdPb.Should().Be("PB_NEW");

            var qd = await _context.QuyetDinhNhanSus.FindAsync("QD01");
            qd.Should().NotBeNull();
            qd!.IdChucVuMoi.Should().Be("CV1");
            qd.LoaiQuyetDinh.Should().Be("Điều chuyển công tác");
        }
    }
}
