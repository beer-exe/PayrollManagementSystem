using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Features.DonNghi.Commands.HuyDonNghiDaDuyet;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.DonNghi.Commands.HuyDonNghiDaDuyet
{
    public class HuyDonNghiDaDuyetCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly HuyDonNghiDaDuyetCommandHandler _handler;

        public HuyDonNghiDaDuyetCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new HuyDonNghiDaDuyetCommandHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_DonNghiNotFound_ThrowsApiException()
        {
            var command = new HuyDonNghiDaDuyetCommand { Id = Guid.NewGuid() };
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Không tìm thấy đơn nghỉ");
        }

        [Fact]
        public async Task Handle_NotDaDuyet_ThrowsApiException()
        {
            var donNghi = new Domain.Models.DonNghi { Id = Guid.NewGuid(), CccdNhanVien = "001", TrangThai = TrangThaiDonNghi.CHO_DUYET, LyDo = "Test" };
            _context.DonNghis.Add(donNghi);
            await _context.SaveChangesAsync();

            var command = new HuyDonNghiDaDuyetCommand { Id = donNghi.Id };
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Chỉ có thể hủy đơn đang ở trạng thái 'Đã duyệt'");
        }

        [Fact]
        public async Task Handle_PastStartDate_ThrowsApiException()
        {
            var donNghi = new Domain.Models.DonNghi 
            { 
                Id = Guid.NewGuid(), CccdNhanVien = "001", TrangThai = TrangThaiDonNghi.DA_DUYET, LyDo = "Test",
                NgayBatDau = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)) // Yesterday
            };
            _context.DonNghis.Add(donNghi);
            await _context.SaveChangesAsync();

            var command = new HuyDonNghiDaDuyetCommand { Id = donNghi.Id };
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("trước khi ngày nghỉ bắt đầu");
        }

        [Fact]
        public async Task Handle_ValidRequest_CancelsAndRevertsNgayPhep()
        {
            // Arrange
            var futureDate = DateOnly.FromDateTime(DateTime.Today.AddDays(5));
            var donNghi = new Domain.Models.DonNghi 
            { 
                Id = Guid.NewGuid(), 
                CccdNhanVien = "001",
                TrangThai = TrangThaiDonNghi.DA_DUYET, LyDo = "Test",
                LoaiNghi = LoaiNghi.NGHI_PHEP_NAM,
                NgayBatDau = futureDate,
                NgayKetThuc = futureDate,
                SoNgayNghi = 1
            };
            
            var ngayPhep = new NgayPhepNhanVien { CccdNhanVien = "001", Nam = futureDate.Year, TongNgayPhep = 12, DaSuDung = 5 };
            
            _context.DonNghis.Add(donNghi);
            _context.NgayPhepNhanViens.Add(ngayPhep);
            await _context.SaveChangesAsync();

            var command = new HuyDonNghiDaDuyetCommand { Id = donNghi.Id };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            
            var dbDonNghi = await _context.DonNghis.FindAsync(donNghi.Id);
            dbDonNghi!.TrangThai.Should().Be(TrangThaiDonNghi.TU_CHOI);
            
            var dbNgayPhep = await _context.NgayPhepNhanViens.FirstOrDefaultAsync(n => n.CccdNhanVien == "001");
            dbNgayPhep!.DaSuDung.Should().Be(4); // Reverted 1 day
        }
    }
}
