using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Features.WorkShifts.Commands.CreateCaLamViec;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;

namespace PayrollManagementSystem.UnitTests.Application.Features.WorkShifts.Commands.CreateCaLamViec
{
    public class CreateCaLamViecCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly CreateCaLamViecCommandHandler _handler;

        public CreateCaLamViecCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new CreateCaLamViecCommandHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_ValidRequest_CreatesShiftWithBreaks()
        {
            var command = new CreateCaLamViecCommand
            {
                TenCa = "Ca sáng",
                GioBatDau = "08:00:00",
                GioKetThuc = "12:00:00",
                XuyenNgay = false,
                HeSoLuong = 1.0m,
                TrangThai = true,
                KhungGioNghis = new List<CreateKhungGioNghiCommand>
                {
                    new CreateKhungGioNghiCommand
                    {
                        TenKhoangNghi = "Giải lao",
                        GioBatDau = "10:00:00",
                        GioKetThuc = "10:15:00",
                        TinhVaoGioLam = true
                    }
                }
            };

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            result.Message.Should().Contain("Tạo ca làm việc thành công");

            var savedCa = await _context.CaLamViecs.Include(c => c.KhungGioNghis).FirstOrDefaultAsync(c => c.Id == result.Data);
            savedCa.Should().NotBeNull();
            savedCa!.TenCa.Should().Be("Ca sáng");
            savedCa.KhungGioNghis.Should().HaveCount(1);
            savedCa.KhungGioNghis.First().TenKhoangNghi.Should().Be("Giải lao");
        }
    }
}
