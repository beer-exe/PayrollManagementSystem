using FluentAssertions;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Features.WorkSchedule.Commands.CreateLichLamViec;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;

namespace PayrollManagementSystem.UnitTests.Application.Features.WorkSchedule.Commands.CreateLichLamViec
{
    public class CreateLichLamViecCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly CreateLichLamViecCommandHandler _handler;

        public CreateLichLamViecCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new CreateLichLamViecCommandHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_YearAlreadyExists_ThrowsApiException()
        {
            _context.LichLamViecs.Add(new LichLamViec { IdLich = Guid.NewGuid(), Nam = 2024 });
            await _context.SaveChangesAsync();

            var command = new CreateLichLamViecCommand { Nam = 2024 };

            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Lịch làm việc năm 2024 đã tồn tại");
        }

        [Fact]
        public async Task Handle_DefaultShiftSelectedButNotFound_ThrowsApiException()
        {
            var command = new CreateLichLamViecCommand { Nam = 2024, UseDefaultShift = true, DefaultShiftId = Guid.NewGuid() };

            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Không tìm thấy ca làm việc mặc định");
        }

        [Fact]
        public async Task Handle_ValidRequest_NoDefaultShift_CreatesScheduleWith8Hours()
        {
            var command = new CreateLichLamViecCommand { Nam = 2024, UseDefaultShift = false };

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            result.Message.Should().Contain("Tổng 366 ngày"); // 2024 is a leap year

            var chiTiets = _context.ChiTietLichLamViecs.Where(c => c.IdLich == result.Data).ToList();
            chiTiets.Should().HaveCount(366);

            var ngayLamViec = chiTiets.First(c => c.LoaiNgay == LoaiNgay.NGAY_LAM_VIEC);
            ngayLamViec.SoGioLam.Should().Be(8m);
            ngayLamViec.IdCaLamViecMacDinh.Should().BeNull();
        }

        [Fact]
        public async Task Handle_ValidRequest_WithDefaultShift_CreatesScheduleWithShiftHours()
        {
            var shiftId = Guid.NewGuid();
            _context.CaLamViecs.Add(new CaLamViec
            {
                Id = shiftId,
                TenCa = "Ca hành chính",
                GioBatDau = new TimeSpan(8, 0, 0),
                GioKetThuc = new TimeSpan(17, 0, 0),
                KhungGioNghis = new List<KhungGioNghi>
                {
                    new KhungGioNghi { GioBatDau = new TimeSpan(12, 0, 0), GioKetThuc = new TimeSpan(13, 0, 0), TenKhoangNghi = "Nghỉ trưa" }
                }
            });
            await _context.SaveChangesAsync();

            var command = new CreateLichLamViecCommand { Nam = 2023, UseDefaultShift = true, DefaultShiftId = shiftId };

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            result.Message.Should().Contain("Tổng 365 ngày"); // 2023 is not a leap year

            var chiTiets = _context.ChiTietLichLamViecs.Where(c => c.IdLich == result.Data).ToList();
            chiTiets.Should().HaveCount(365);

            var ngayLamViec = chiTiets.First(c => c.LoaiNgay == LoaiNgay.NGAY_LAM_VIEC);
            ngayLamViec.SoGioLam.Should().Be(8m); // (17-8) - 1 = 8 hours
            ngayLamViec.IdCaLamViecMacDinh.Should().Be(shiftId);
        }
    }
}
