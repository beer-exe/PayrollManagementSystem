using FluentAssertions;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Features.WorkSchedule.Commands.UpdateChiTietLichLamViec;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.WorkSchedule.Commands.UpdateChiTietLichLamViec
{
    public class UpdateChiTietLichLamViecCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly UpdateChiTietLichLamViecCommandHandler _handler;

        public UpdateChiTietLichLamViecCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new UpdateChiTietLichLamViecCommandHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_DetailNotFound_ThrowsApiException()
        {
            var command = new UpdateChiTietLichLamViecCommand { IdChiTiet = Guid.NewGuid(), LoaiNgay = "Ngày làm việc" };

            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Không tìm thấy chi tiết lịch làm việc");
        }

        [Fact]
        public async Task Handle_ScheduleExpired_ThrowsApiException()
        {
            var lich = new LichLamViec { IdLich = Guid.NewGuid(), Nam = 2024, TrangThai = TrangThaiLichLamViec.HET_HIEU_LUC };
            var chiTiet = new ChiTietLichLamViec { Id = Guid.NewGuid(), IdLich = lich.IdLich, Ngay = DateOnly.FromDateTime(DateTime.Today.AddDays(1)), Thu = "Thứ Hai", LichLamViec = lich };
            _context.LichLamViecs.Add(lich);
            _context.ChiTietLichLamViecs.Add(chiTiet);
            await _context.SaveChangesAsync();

            var command = new UpdateChiTietLichLamViecCommand { IdChiTiet = chiTiet.Id, LoaiNgay = "Ngày làm việc" };

            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("đã hết hiệu lực");
        }

        [Fact]
        public async Task Handle_PastDate_ThrowsApiException()
        {
            var lich = new LichLamViec { IdLich = Guid.NewGuid(), Nam = 2024, TrangThai = TrangThaiLichLamViec.HIEU_LUC };
            var chiTiet = new ChiTietLichLamViec { Id = Guid.NewGuid(), IdLich = lich.IdLich, Ngay = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)), Thu = "Thứ Hai", LichLamViec = lich };
            _context.LichLamViecs.Add(lich);
            _context.ChiTietLichLamViecs.Add(chiTiet);
            await _context.SaveChangesAsync();

            var command = new UpdateChiTietLichLamViecCommand { IdChiTiet = chiTiet.Id, LoaiNgay = "Ngày làm việc" };

            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("trong quá khứ");
        }

        [Fact]
        public async Task Handle_InvalidDayType_ThrowsApiException()
        {
            var lich = new LichLamViec { IdLich = Guid.NewGuid(), Nam = 2024, TrangThai = TrangThaiLichLamViec.HIEU_LUC };
            var chiTiet = new ChiTietLichLamViec { Id = Guid.NewGuid(), IdLich = lich.IdLich, Ngay = DateOnly.FromDateTime(DateTime.Today.AddDays(1)), Thu = "Thứ Hai", LichLamViec = lich };
            _context.LichLamViecs.Add(lich);
            _context.ChiTietLichLamViecs.Add(chiTiet);
            await _context.SaveChangesAsync();

            var command = new UpdateChiTietLichLamViecCommand { IdChiTiet = chiTiet.Id, LoaiNgay = "Ngày xàm" };

            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Loại ngày không hợp lệ");
        }

        [Fact]
        public async Task Handle_ConflictingLeavesForWeekendOrHoliday_ThrowsApiException()
        {
            var targetDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
            var lich = new LichLamViec { IdLich = Guid.NewGuid(), Nam = 2024, TrangThai = TrangThaiLichLamViec.HIEU_LUC };
            var chiTiet = new ChiTietLichLamViec { Id = Guid.NewGuid(), IdLich = lich.IdLich, Ngay = targetDate, Thu = "Thứ Hai", LoaiNgay = LoaiNgay.NGAY_LAM_VIEC, LichLamViec = lich };
            _context.LichLamViecs.Add(lich);
            _context.ChiTietLichLamViecs.Add(chiTiet);

            var donNghi = new PayrollManagementSystem.Domain.Models.DonNghi { Id = Guid.NewGuid(), CccdNhanVien = "123", NgayBatDau = targetDate, NgayKetThuc = targetDate, TrangThai = TrangThaiDonNghi.DA_DUYET, LyDo = "x" };
            _context.DonNghis.Add(donNghi);
            await _context.SaveChangesAsync();

            var command = new UpdateChiTietLichLamViecCommand { IdChiTiet = chiTiet.Id, LoaiNgay = "Nghỉ lễ" };

            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Có đơn xin nghỉ phép của nhân viên trong ngày này");
        }

        [Fact]
        public async Task Handle_ValidRequest_UpdateToWorkingDayWithShift_UpdatesCorrectly()
        {
            var shiftId = Guid.NewGuid();
            _context.CaLamViecs.Add(new CaLamViec
            {
                Id = shiftId,
                TenCa = "Ca",
                GioBatDau = new TimeSpan(8, 0, 0),
                GioKetThuc = new TimeSpan(12, 0, 0),
            });

            var targetDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
            var lich = new LichLamViec { IdLich = Guid.NewGuid(), Nam = 2024, TrangThai = TrangThaiLichLamViec.HIEU_LUC };
            var chiTiet = new ChiTietLichLamViec { Id = Guid.NewGuid(), IdLich = lich.IdLich, Ngay = targetDate, Thu = "Thứ Hai", LoaiNgay = LoaiNgay.NGHI_LE, LichLamViec = lich };
            _context.LichLamViecs.Add(lich);
            _context.ChiTietLichLamViecs.Add(chiTiet);
            await _context.SaveChangesAsync();

            var command = new UpdateChiTietLichLamViecCommand { IdChiTiet = chiTiet.Id, LoaiNgay = "Ngày làm việc", IdCaLamViecMacDinh = shiftId };

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            chiTiet.LoaiNgay.Should().Be(LoaiNgay.NGAY_LAM_VIEC);
            chiTiet.SoGioLam.Should().Be(4m); // 12 - 8
            chiTiet.IdCaLamViecMacDinh.Should().Be(shiftId);
            chiTiet.TenNgayNghi.Should().BeNull();
        }

        [Fact]
        public async Task Handle_ValidRequest_UpdateToHoliday_UpdatesCorrectly()
        {
            var targetDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
            var lich = new LichLamViec { IdLich = Guid.NewGuid(), Nam = 2024, TrangThai = TrangThaiLichLamViec.HIEU_LUC };
            var chiTiet = new ChiTietLichLamViec { Id = Guid.NewGuid(), IdLich = lich.IdLich, Ngay = targetDate, Thu = "Thứ Hai", LoaiNgay = LoaiNgay.NGAY_LAM_VIEC, LichLamViec = lich };
            _context.LichLamViecs.Add(lich);
            _context.ChiTietLichLamViecs.Add(chiTiet);
            await _context.SaveChangesAsync();

            var command = new UpdateChiTietLichLamViecCommand { IdChiTiet = chiTiet.Id, LoaiNgay = "Nghỉ lễ", TenNgayNghi = "Tết" };

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            chiTiet.LoaiNgay.Should().Be(LoaiNgay.NGHI_LE);
            chiTiet.SoGioLam.Should().Be(0m);
            chiTiet.IdCaLamViecMacDinh.Should().BeNull();
            chiTiet.TenNgayNghi.Should().Be("Tết");
        }
    }
}
