using FluentAssertions;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Features.Employees.Commands.CreateEmployee;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.Employees.Commands.CreateEmployee
{
    public class CreateEmployeeCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly CreateEmployeeCommandHandler _handler;

        public CreateEmployeeCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new CreateEmployeeCommandHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_EmployeeExists_ThrowsApiException()
        {
            _context.NhanViens.Add(new NhanVien { Cccd = "001", HoTen = "Test" });
            await _context.SaveChangesAsync();

            var command = new CreateEmployeeCommand { Cccd = "001" };
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("đã tồn tại");
        }

        [Fact]
        public async Task Handle_ContractExists_ThrowsApiException()
        {
            _context.HopDongLaoDongs.Add(new HopDongLaoDong { SoHopDong = "HD01", Cccd = "001", LoaiHopDong = "Xác định thời hạn" });
            await _context.SaveChangesAsync();

            var command = new CreateEmployeeCommand { Cccd = "002", SoHopDong = "HD01" };
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Số hợp đồng");
            exception.Message.Should().Contain("đã tồn tại");
        }

        [Fact]
        public async Task Handle_DecisionExists_ThrowsApiException()
        {
            _context.QuyetDinhNhanSus.Add(new QuyetDinhNhanSu { SoQuyetDinh = "QD01", Cccd = "002", LoaiQuyetDinh = "Tuyển dụng", IdChucVuMoi = "CV01" });
            await _context.SaveChangesAsync();

            var command = new CreateEmployeeCommand { Cccd = "002", SoHopDong = "HD02", SoQuyetDinh = "QD01" };
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Số quyết định");
            exception.Message.Should().Contain("đã tồn tại");
        }

        [Fact]
        public async Task Handle_DepartmentNotFound_ThrowsApiException()
        {
            var command = new CreateEmployeeCommand { Cccd = "002", SoHopDong = "HD02", SoQuyetDinh = "QD02", IdPb = "PB01" };
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Phòng ban");
            exception.Message.Should().Contain("không tồn tại");
        }

        [Fact]
        public async Task Handle_PositionNotFound_ThrowsApiException()
        {
            _context.PhongBans.Add(new PhongBan { IdPb = "PB01", TenPb = "Phòng IT" });
            await _context.SaveChangesAsync();

            var command = new CreateEmployeeCommand { Cccd = "002", SoHopDong = "HD02", SoQuyetDinh = "QD02", IdPb = "PB01", IdChucVu = "CV01" };
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Chức vụ");
            exception.Message.Should().Contain("không tồn tại");
        }
        
        [Fact]
        public async Task Handle_BacLuongNotFound_ThrowsApiException()
        {
            _context.PhongBans.Add(new PhongBan { IdPb = "PB01", TenPb = "Phòng IT" });
            _context.ChucVus.Add(new ChucVu { IdChucVu = "CV01", TenChucVu = "Nhân viên", IdPhongBan = "PB01" });
            await _context.SaveChangesAsync();

            var command = new CreateEmployeeCommand { Cccd = "002", SoHopDong = "HD02", SoQuyetDinh = "QD02", IdPb = "PB01", IdChucVu = "CV01", IdBacLuong = "BL01" };
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Bậc lương");
            exception.Message.Should().Contain("không tồn tại");
        }

        [Fact]
        public async Task Handle_ValidRequest_CreatesEmployee()
        {
            // Arrange
            _context.PhongBans.Add(new PhongBan { IdPb = "PB01", TenPb = "Phòng IT" });
            _context.ChucVus.Add(new ChucVu { IdChucVu = "CV01", TenChucVu = "Nhân viên", IdPhongBan = "PB01" });
            _context.BacLuongs.Add(new BacLuong { IdBacLuong = "BL01", TenBacLuong = "Bậc 1", IdNgachLuong = "NL01" });
            await _context.SaveChangesAsync();

            var command = new CreateEmployeeCommand 
            { 
                Cccd = "001",
                SoHopDong = "HD01",
                SoQuyetDinh = "QD01",
                IdPb = "PB01",
                IdChucVu = "CV01",
                IdBacLuong = "BL01",
                HoTen = "Test NV",
                LoaiHopDong = "Xác định thời hạn",
                LuongCoBan = 10000000
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            result.Data.Should().Be("001");

            var nhanVien = await _context.NhanViens.FindAsync("001");
            nhanVien.Should().NotBeNull();
            nhanVien!.HoTen.Should().Be("Test NV");
            nhanVien.TrangThai.Should().Be(TrangThaiNhanVien.DANG_LAM_VIEC);

            var hopDong = await _context.HopDongLaoDongs.FindAsync("HD01");
            hopDong.Should().NotBeNull();
            hopDong!.Cccd.Should().Be("001");

            var quyetDinh = await _context.QuyetDinhNhanSus.FindAsync("QD01");
            quyetDinh.Should().NotBeNull();
            quyetDinh!.Cccd.Should().Be("001");
            quyetDinh.IdChucVuMoi.Should().Be("CV01");
        }
    }
}
