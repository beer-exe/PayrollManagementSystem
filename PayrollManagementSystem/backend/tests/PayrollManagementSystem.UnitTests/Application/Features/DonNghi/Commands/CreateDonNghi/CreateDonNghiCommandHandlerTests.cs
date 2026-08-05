using FluentAssertions;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Features.DonNghi.Commands.CreateDonNghi;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.DonNghi.Commands.CreateDonNghi
{
    public class CreateDonNghiCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly CreateDonNghiCommandHandler _handler;

        public CreateDonNghiCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new CreateDonNghiCommandHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_InvalidLoaiNghi_ThrowsApiException()
        {
            var command = new CreateDonNghiCommand { LoaiNghi = "INVALID" };
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Loại nghỉ không hợp lệ");
        }

        [Fact]
        public async Task Handle_EmployeeNotFound_ThrowsApiException()
        {
            var command = new CreateDonNghiCommand { LoaiNghi = "NGHI_PHEP_NAM", CccdNhanVien = "001" };
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Nhân viên không tồn tại");
        }

        [Fact]
        public async Task Handle_NoLichLamViec_ThrowsApiException()
        {
            _context.NhanViens.Add(new NhanVien { Cccd = "001", HoTen = "Test NV" });
            await _context.SaveChangesAsync();

            var command = new CreateDonNghiCommand 
            { 
                LoaiNghi = "NGHI_PHEP_NAM", 
                CccdNhanVien = "001", 
                NgayBatDau = new DateOnly(2025, 1, 1), 
                NgayKetThuc = new DateOnly(2025, 1, 2), 
                SoNgayNghi = 2 
            };

            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Chưa có lịch làm việc nào được tạo");
        }

        [Fact]
        public async Task Handle_ValidRequest_CreatesDonNghi()
        {
            // Arrange
            _context.NhanViens.Add(new NhanVien { Cccd = "001", HoTen = "Test NV" });
            var lichId = Guid.NewGuid();
            _context.LichLamViecs.Add(new LichLamViec { IdLich = lichId, Nam = 2025 });
            _context.ChiTietLichLamViecs.Add(new ChiTietLichLamViec { IdLich = lichId, Ngay = new DateOnly(2025, 1, 1), Thu = "T4", LoaiNgay = LoaiNgay.NGAY_LAM_VIEC });
            await _context.SaveChangesAsync();

            var command = new CreateDonNghiCommand 
            { 
                LoaiNghi = "NGHI_PHEP_NAM", 
                CccdNhanVien = "001", 
                NgayBatDau = new DateOnly(2025, 1, 1), 
                NgayKetThuc = new DateOnly(2025, 1, 1), 
                SoNgayNghi = 1,
                LyDo = "Test"
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            
            var donNghi = await _context.DonNghis.FindAsync(result.Data);
            donNghi.Should().NotBeNull();
            donNghi!.TrangThai.Should().Be(TrangThaiDonNghi.CHO_DUYET);
            donNghi.SoNgayNghi.Should().Be(1);
        }
    }
}
