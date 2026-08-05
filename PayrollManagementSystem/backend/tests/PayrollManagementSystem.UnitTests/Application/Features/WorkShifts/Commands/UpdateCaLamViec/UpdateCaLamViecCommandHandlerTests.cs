using FluentAssertions;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Features.WorkShifts.Commands.UpdateCaLamViec;
using PayrollManagementSystem.Application.Features.WorkShifts.DTOs;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using Xunit;
using Microsoft.EntityFrameworkCore;

namespace PayrollManagementSystem.UnitTests.Application.Features.WorkShifts.Commands.UpdateCaLamViec
{
    public class UpdateCaLamViecCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly UpdateCaLamViecCommandHandler _handler;

        public UpdateCaLamViecCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new UpdateCaLamViecCommandHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_ShiftNotFound_ThrowsApiException()
        {
            var command = new UpdateCaLamViecCommand { Id = Guid.NewGuid() };

            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Không tìm thấy ca làm việc");
        }

        [Fact]
        public async Task Handle_ShiftUsedInPast_AndCoreModified_ThrowsApiException()
        {
            var shiftId = Guid.NewGuid();
            var shift = new CaLamViec 
            { 
                Id = shiftId, 
                TenCa = "Ca 1", 
                GioBatDau = new TimeSpan(8,0,0), 
                GioKetThuc = new TimeSpan(12,0,0) 
            };
            
            var lich = new LichLamViec { IdLich = Guid.NewGuid(), Nam = 2024 };

            var scheduleDetail = new ChiTietLichLamViec 
            { 
                Id = Guid.NewGuid(), 
                IdLich = lich.IdLich,
                LichLamViec = lich,
                Ngay = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)), // Past date
                Thu = "Hai", 
                IdCaLamViecMacDinh = shiftId 
            };
            
            _context.CaLamViecs.Add(shift);
            _context.LichLamViecs.Add(lich);
            _context.ChiTietLichLamViecs.Add(scheduleDetail);
            await _context.SaveChangesAsync();

            var command = new UpdateCaLamViecCommand 
            { 
                Id = shiftId, 
                TenCa = "Ca 1 Sửa", 
                GioBatDau = "09:00:00", // Core modified
                GioKetThuc = "12:00:00",
                KhungGioNghis = new List<UpdateKhungGioNghiCommand>()
            };

            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Ca làm việc này đã có dữ liệu chấm công nên không thể thay đổi giờ giấc hoặc hệ số lương.");
        }

        [Fact]
        public async Task Handle_ShiftUsedInPast_AndNotCoreModified_UpdatesNameOnly()
        {
            var shiftId = Guid.NewGuid();
            var shift = new CaLamViec 
            { 
                Id = shiftId, 
                TenCa = "Ca 1", 
                GioBatDau = new TimeSpan(8,0,0), 
                GioKetThuc = new TimeSpan(12,0,0),
                HeSoLuong = 1.0m,
                XuyenNgay = false
            };
            
            var lich = new LichLamViec { IdLich = Guid.NewGuid(), Nam = 2024 };

            var scheduleDetail = new ChiTietLichLamViec 
            { 
                Id = Guid.NewGuid(), 
                IdLich = lich.IdLich,
                LichLamViec = lich,
                Ngay = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)), 
                Thu = "Hai", 
                IdCaLamViecMacDinh = shiftId 
            };
            
            _context.CaLamViecs.Add(shift);
            _context.LichLamViecs.Add(lich);
            _context.ChiTietLichLamViecs.Add(scheduleDetail);
            await _context.SaveChangesAsync();

            var command = new UpdateCaLamViecCommand 
            { 
                Id = shiftId, 
                TenCa = "Ca 1 Sửa Tên", // Only name changes
                GioBatDau = "08:00:00", 
                GioKetThuc = "12:00:00",
                HeSoLuong = 1.0m,
                XuyenNgay = false,
                KhungGioNghis = new List<UpdateKhungGioNghiCommand>()
            };

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            
            var updatedShift = await _context.CaLamViecs.FindAsync(shiftId);
            updatedShift!.TenCa.Should().Be("Ca 1 Sửa Tên");
        }

        [Fact]
        public async Task Handle_ValidRequest_NotUsedInPast_UpdatesAllAndFutureSchedules()
        {
            var shiftId = Guid.NewGuid();
            var breakId = Guid.NewGuid();
            var shift = new CaLamViec 
            { 
                Id = shiftId, 
                TenCa = "Ca 1", 
                GioBatDau = new TimeSpan(8,0,0), 
                GioKetThuc = new TimeSpan(12,0,0),
                KhungGioNghis = new List<KhungGioNghi>
                {
                    new KhungGioNghi
                    {
                        Id = breakId,
                        IdCaLamViec = shiftId,
                        TenKhoangNghi = "Nghỉ sáng",
                        GioBatDau = new TimeSpan(10,0,0),
                        GioKetThuc = new TimeSpan(10,30,0),
                        TinhVaoGioLam = true
                    }
                }
            };
            
            _context.CaLamViecs.Add(shift);
            await _context.SaveChangesAsync();

            var command = new UpdateCaLamViecCommand 
            { 
                Id = shiftId, 
                TenCa = "Ca 1 Mới",
                GioBatDau = "08:00:00", 
                GioKetThuc = "17:00:00",
                HeSoLuong = 1.0m,
                XuyenNgay = false,
                KhungGioNghis = new List<UpdateKhungGioNghiCommand>
                {
                    new UpdateKhungGioNghiCommand
                    {
                        Id = breakId, // Pass the existing ID
                        TenKhoangNghi = "Nghỉ trưa",
                        GioBatDau = "12:00:00",
                        GioKetThuc = "13:00:00",
                        TinhVaoGioLam = false
                    }
                }
            };

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            
            var updatedShift = await _context.CaLamViecs.Include(c => c.KhungGioNghis).FirstOrDefaultAsync(c => c.Id == shiftId);
            updatedShift!.GioKetThuc.Should().Be(new TimeSpan(17,0,0));
            updatedShift.KhungGioNghis.Should().HaveCount(1);
        }
    }
}
