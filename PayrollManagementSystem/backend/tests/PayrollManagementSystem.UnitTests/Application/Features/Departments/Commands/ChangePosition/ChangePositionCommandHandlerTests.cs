using FluentAssertions;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Features.Departments.Commands.ChangePosition;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;

namespace PayrollManagementSystem.UnitTests.Application.Features.Departments.Commands.ChangePosition
{
    public class ChangePositionCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly ChangePositionCommandHandler _handler;

        public ChangePositionCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new ChangePositionCommandHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_EmployeeNotFound_ThrowsApiException()
        {
            var command = new ChangePositionCommand { Cccd = "001", SoQuyetDinh = "QD01", IdChucVuMoi = "CV2", IdBacLuongMoi = "B02", NgayHieuLuc = DateTime.Now };

            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Nhân viên không tồn tại");
        }

        [Fact]
        public async Task Handle_ValidRequest_CreatesQuyetDinhAndExpiresOld()
        {
            // Arrange
            _context.NhanViens.Add(new NhanVien { Cccd = "001", HoTen = "Test NV" });

            var oldQd = new QuyetDinhNhanSu
            {
                SoQuyetDinh = "QD01",
                Cccd = "001",
                IdChucVuMoi = "CV1",
                IdBacLuongMoi = "B1",
                TrangThai = TrangThaiQuyetDinh.HIEU_LUC,
                NgayHieuLuc = DateOnly.FromDateTime(DateTime.Today.AddDays(-10)),
                LoaiQuyetDinh = "Test"
            };
            _context.QuyetDinhNhanSus.Add(oldQd);
            await _context.SaveChangesAsync();

            var command = new ChangePositionCommand
            {
                Cccd = "001",
                SoQuyetDinh = "QD02",
                IdChucVuMoi = "CV2",
                IdBacLuongMoi = "B2",
                NgayHieuLuc = DateTime.Today
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();

            var newQd = await _context.QuyetDinhNhanSus.FindAsync("QD02");
            newQd.Should().NotBeNull();
            newQd!.IdChucVuMoi.Should().Be("CV2");
            newQd.IdBacLuongMoi.Should().Be("B2");
            newQd.LoaiQuyetDinh.Should().Be("Thay đổi chức vụ");

            var dbOldQd = await _context.QuyetDinhNhanSus.FindAsync("QD01");
            dbOldQd!.TrangThai.Should().Be(TrangThaiQuyetDinh.HET_HAN);
        }
    }
}
