using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Features.DonNghi.Commands.DuyetDonNghi;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.DonNghi.Commands.DuyetDonNghi
{
    public class DuyetDonNghiCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly DuyetDonNghiCommandHandler _handler;

        public DuyetDonNghiCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new DuyetDonNghiCommandHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_DonNghiNotFound_ThrowsApiException()
        {
            var command = new DuyetDonNghiCommand { Id = Guid.NewGuid(), CccdNguoiDuyet = "ADMIN" };
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Không tìm thấy đơn nghỉ");
        }

        [Fact]
        public async Task Handle_NotChoDuyet_ThrowsApiException()
        {
            var donNghi = new Domain.Models.DonNghi { Id = Guid.NewGuid(), CccdNhanVien = "001", TrangThai = TrangThaiDonNghi.DA_DUYET, LyDo = "Test" };
            _context.DonNghis.Add(donNghi);
            await _context.SaveChangesAsync();

            var command = new DuyetDonNghiCommand { Id = donNghi.Id, CccdNguoiDuyet = "ADMIN" };
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Chỉ có thể duyệt đơn đang ở trạng thái 'Chờ duyệt'");
        }

        [Fact]
        public async Task Handle_NotEnoughNgayPhep_ThrowsApiException()
        {
            var donNghi = new Domain.Models.DonNghi 
            { 
                Id = Guid.NewGuid(), 
                CccdNhanVien = "001",
                TrangThai = TrangThaiDonNghi.CHO_DUYET, LyDo = "Test",
                LoaiNghi = LoaiNghi.NGHI_PHEP_NAM,
                NgayBatDau = new DateOnly(2025, 1, 1),
                SoNgayNghi = 5
            };
            
            var ngayPhep = new NgayPhepNhanVien { CccdNhanVien = "001", Nam = 2025, TongNgayPhep = 12, DaSuDung = 10 }; // ConLai = 2

            _context.DonNghis.Add(donNghi);
            _context.NgayPhepNhanViens.Add(ngayPhep);
            await _context.SaveChangesAsync();

            var command = new DuyetDonNghiCommand { Id = donNghi.Id, CccdNguoiDuyet = "ADMIN" };
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("không đủ cho đơn này");
        }

        [Fact]
        public async Task Handle_ValidRequest_DeductsNgayPhepAndCreatesChamCong()
        {
            // Arrange
            var donNghi = new Domain.Models.DonNghi 
            { 
                Id = Guid.NewGuid(), 
                CccdNhanVien = "001",
                TrangThai = TrangThaiDonNghi.CHO_DUYET, LyDo = "Test",
                LoaiNghi = LoaiNghi.NGHI_PHEP_NAM,
                NgayBatDau = new DateOnly(2025, 1, 1),
                NgayKetThuc = new DateOnly(2025, 1, 2),
                SoNgayNghi = 2
            };
            
            var ngayPhep = new NgayPhepNhanVien { CccdNhanVien = "001", Nam = 2025, TongNgayPhep = 12, DaSuDung = 0 };
            
            _context.DonNghis.Add(donNghi);
            _context.NgayPhepNhanViens.Add(ngayPhep);
            var lichId = Guid.NewGuid();
            _context.ChiTietLichLamViecs.AddRange(
                new ChiTietLichLamViec { IdLich = lichId, Ngay = new DateOnly(2025, 1, 1), Thu = "T4", LoaiNgay = LoaiNgay.NGAY_LAM_VIEC },
                new ChiTietLichLamViec { IdLich = lichId, Ngay = new DateOnly(2025, 1, 2), Thu = "T5", LoaiNgay = LoaiNgay.NGAY_LAM_VIEC }
            );
            await _context.SaveChangesAsync();

            var command = new DuyetDonNghiCommand { Id = donNghi.Id, CccdNguoiDuyet = "ADMIN" };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            
            var dbDonNghi = await _context.DonNghis.FindAsync(donNghi.Id);
            dbDonNghi!.TrangThai.Should().Be(TrangThaiDonNghi.DA_DUYET);
            dbDonNghi.CccdNguoiDuyet.Should().Be("ADMIN");

            var dbNgayPhep = await _context.NgayPhepNhanViens.FirstOrDefaultAsync(n => n.CccdNhanVien == "001");
            dbNgayPhep!.DaSuDung.Should().Be(2);

            var chamCongs = _context.ChamCongs.Where(c => c.CccdNhanVien == "001").ToList();
            chamCongs.Should().HaveCount(2);
            chamCongs.All(c => c.LoaiNgayCong == LoaiNgayCong.VANG_CO_PHEP).Should().BeTrue();
        }
    }
}
