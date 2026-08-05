using FluentAssertions;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Features.WorkShifts.Commands.DeleteCaLamViec;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.WorkShifts.Commands.DeleteCaLamViec
{
    public class DeleteCaLamViecCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly DeleteCaLamViecCommandHandler _handler;

        public DeleteCaLamViecCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new DeleteCaLamViecCommandHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_ShiftNotFound_ThrowsApiException()
        {
            var command = new DeleteCaLamViecCommand { Id = Guid.NewGuid() };

            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Không tìm thấy ca làm việc");
        }

        [Fact]
        public async Task Handle_ShiftUsedInSchedule_ThrowsApiException()
        {
            var shift = new CaLamViec { Id = Guid.NewGuid(), TenCa = "Ca 1" };
            var lich = new LichLamViec { IdLich = Guid.NewGuid(), Nam = 2024 };
            var scheduleDetail = new ChiTietLichLamViec { Id = Guid.NewGuid(), IdLich = lich.IdLich, LichLamViec = lich, Ngay = new DateOnly(2024,1,1), Thu = "Hai", IdCaLamViecMacDinh = shift.Id };
            _context.CaLamViecs.Add(shift);
            _context.LichLamViecs.Add(lich);
            _context.ChiTietLichLamViecs.Add(scheduleDetail);
            await _context.SaveChangesAsync();

            var command = new DeleteCaLamViecCommand { Id = shift.Id };

            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("đã được gán vào lịch làm việc hoặc phân công ca");
        }

        [Fact]
        public async Task Handle_ShiftUsedInAssignment_ThrowsApiException()
        {
            var shift = new CaLamViec { Id = Guid.NewGuid(), TenCa = "Ca 1" };
            var assignment = new PhanCongCa { IdPhanCong = Guid.NewGuid(), CccdNhanVien = "123", NgayLamViec = new DateOnly(2024,1,1), IdCaLamViec = shift.Id };
            _context.CaLamViecs.Add(shift);
            _context.PhanCongCas.Add(assignment);
            await _context.SaveChangesAsync();

            var command = new DeleteCaLamViecCommand { Id = shift.Id };

            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("đã được gán vào lịch làm việc hoặc phân công ca");
        }

        [Fact]
        public async Task Handle_ValidRequest_SoftDeletesShiftAndBreaks()
        {
            var shiftId = Guid.NewGuid();
            var shift = new CaLamViec 
            { 
                Id = shiftId, 
                TenCa = "Ca 1",
                KhungGioNghis = new List<KhungGioNghi> { new KhungGioNghi { Id = Guid.NewGuid(), TenKhoangNghi = "Nghỉ" } }
            };
            _context.CaLamViecs.Add(shift);
            await _context.SaveChangesAsync();

            var command = new DeleteCaLamViecCommand { Id = shiftId };
            var result = await _handler.Handle(command, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            result.Message.Should().Contain("Xóa ca làm việc thành công");

            var deletedShift = await _context.CaLamViecs.FindAsync(shiftId);
            deletedShift!.IsDeleted.Should().BeTrue();
            deletedShift.KhungGioNghis.Should().AllSatisfy(k => k.IsDeleted.Should().BeTrue());
        }
    }
}
