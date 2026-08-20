using FluentAssertions;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Features.Employees.Commands.ChangeEmployeeStatus;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;

namespace PayrollManagementSystem.UnitTests.Application.Features.Employees.Commands.ChangeEmployeeStatus
{
    public class ChangeEmployeeStatusCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly ChangeEmployeeStatusCommandHandler _handler;

        public ChangeEmployeeStatusCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new ChangeEmployeeStatusCommandHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_EmployeeNotFound_ThrowsApiException()
        {
            var command = new ChangeEmployeeStatusCommand { Cccd = "001", TrangThaiMoi = TrangThaiNhanVien.DA_NGHI_VIEC };
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Không tìm thấy nhân viên");
        }

        [Fact]
        public async Task Handle_SameStatus_ReturnsSuccessWithNoChangeMessage()
        {
            _context.NhanViens.Add(new NhanVien { Cccd = "001", HoTen = "Test", TrangThai = TrangThaiNhanVien.DANG_LAM_VIEC });
            await _context.SaveChangesAsync();

            var command = new ChangeEmployeeStatusCommand { Cccd = "001", TrangThaiMoi = TrangThaiNhanVien.DANG_LAM_VIEC };
            var result = await _handler.Handle(command, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            result.Message.Should().Contain("không có thay đổi");
        }

        [Fact]
        public async Task Handle_ValidRequest_ChangesStatusAndLogsHistory()
        {
            _context.NhanViens.Add(new NhanVien { Cccd = "001", HoTen = "Test", TrangThai = TrangThaiNhanVien.DANG_LAM_VIEC });
            await _context.SaveChangesAsync();

            var command = new ChangeEmployeeStatusCommand
            {
                Cccd = "001",
                TrangThaiMoi = TrangThaiNhanVien.DA_NGHI_VIEC,
                LyDo = "Nghỉ hưu",
                NguoiThayDoi = "Admin"
            };

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Succeeded.Should().BeTrue();

            var nv = await _context.NhanViens.FindAsync("001");
            nv!.TrangThai.Should().Be(TrangThaiNhanVien.DA_NGHI_VIEC);
            nv.NgayNghiViec.Should().NotBeNull();

            var log = _context.NhatKyTrangThais.FirstOrDefault(n => n.Cccd == "001");
            log.Should().NotBeNull();
            log!.TrangThaiCu.Should().Be(TrangThaiNhanVien.DANG_LAM_VIEC);
            log.TrangThaiMoi.Should().Be(TrangThaiNhanVien.DA_NGHI_VIEC);
            log.LyDo.Should().Be("Nghỉ hưu");
            log.NguoiThayDoi.Should().Be("Admin");
        }

        [Fact]
        public async Task Handle_ChangeToDangLamViec_ClearsNgayNghiViec()
        {
            _context.NhanViens.Add(new NhanVien { Cccd = "001", HoTen = "Test", TrangThai = TrangThaiNhanVien.DA_NGHI_VIEC, NgayNghiViec = DateOnly.FromDateTime(DateTime.Today) });
            await _context.SaveChangesAsync();

            var command = new ChangeEmployeeStatusCommand { Cccd = "001", TrangThaiMoi = TrangThaiNhanVien.DANG_LAM_VIEC, LyDo = "Test", NguoiThayDoi = "Admin" };
            await _handler.Handle(command, CancellationToken.None);

            var nv = await _context.NhanViens.FindAsync("001");
            nv!.TrangThai.Should().Be(TrangThaiNhanVien.DANG_LAM_VIEC);
            nv.NgayNghiViec.Should().BeNull();
        }
    }
}
