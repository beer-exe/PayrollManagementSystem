using FluentAssertions;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Features.Departments.Commands.AdjustSalary;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.Departments.Commands.AdjustSalary
{
    public class AdjustSalaryCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly AdjustSalaryCommandHandler _handler;

        public AdjustSalaryCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new AdjustSalaryCommandHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_EmployeeNotFound_ThrowsApiException()
        {
            var command = new AdjustSalaryCommand { Cccd = "001", SoQuyetDinh = "QD01", IdBacLuongMoi = "B02", NgayHieuLuc = DateTime.Now };

            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Nhân viên không tồn tại");
        }

        [Fact]
        public async Task Handle_DuplicateSoQuyetDinh_ThrowsApiException()
        {
            _context.NhanViens.Add(new NhanVien { Cccd = "001", HoTen = "Test NV" });
            _context.QuyetDinhNhanSus.Add(new QuyetDinhNhanSu { SoQuyetDinh = "QD01", Cccd = "001", LoaiQuyetDinh = "Test" });
            await _context.SaveChangesAsync();

            var command = new AdjustSalaryCommand { Cccd = "001", SoQuyetDinh = "QD01", IdBacLuongMoi = "B02", NgayHieuLuc = DateTime.Now };

            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("đã tồn tại");
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

            var command = new AdjustSalaryCommand 
            { 
                Cccd = "001", 
                SoQuyetDinh = "QD02", 
                IdBacLuongMoi = "B2", 
                NgayHieuLuc = DateTime.Today 
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();

            var newQd = await _context.QuyetDinhNhanSus.FindAsync("QD02");
            newQd.Should().NotBeNull();
            newQd!.IdChucVuMoi.Should().Be("CV1");
            newQd.IdBacLuongMoi.Should().Be("B2");
            newQd.TrangThai.Should().Be(TrangThaiQuyetDinh.HIEU_LUC);

            var dbOldQd = await _context.QuyetDinhNhanSus.FindAsync("QD01");
            dbOldQd!.TrangThai.Should().Be(TrangThaiQuyetDinh.HET_HAN);
            dbOldQd.NgayHetHan.Should().Be(DateOnly.FromDateTime(DateTime.Today));
        }
    }
}
