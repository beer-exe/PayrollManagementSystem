using FluentAssertions;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Features.PhanCongCas.Commands.UpsertPhanCongCa;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;

namespace PayrollManagementSystem.UnitTests.Application.Features.PhanCongCas.Commands.UpsertPhanCongCa
{
    public class UpsertPhanCongCaCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly UpsertPhanCongCaCommandHandler _handler;

        public UpsertPhanCongCaCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new UpsertPhanCongCaCommandHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_XoaPhanCong_WhenExists_SetsIsDeletedToTrue()
        {
            var date = DateOnly.FromDateTime(DateTime.Today);
            var phanCong = new PhanCongCa
            {
                CccdNhanVien = "001",
                NgayLamViec = date,
                IdCaLamViec = Guid.NewGuid(),
                IsDeleted = false
            };
            _context.PhanCongCas.Add(phanCong);
            await _context.SaveChangesAsync();

            var command = new UpsertPhanCongCaCommand
            {
                CccdNhanVien = "001",
                NgayLamViec = date,
                XoaPhanCong = true
            };

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            result.Message.Should().Contain("Xoá phân công ca thành công");
            phanCong.IsDeleted.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_XoaPhanCong_WhenNotExists_ReturnsSuccess()
        {
            var command = new UpsertPhanCongCaCommand
            {
                CccdNhanVien = "001",
                NgayLamViec = DateOnly.FromDateTime(DateTime.Today),
                XoaPhanCong = true
            };

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            result.Message.Should().Contain("Không có phân công ca nào để xoá");
        }

        [Fact]
        public async Task Handle_InvalidCaLamViec_ThrowsApiException()
        {
            var command = new UpsertPhanCongCaCommand
            {
                CccdNhanVien = "001",
                NgayLamViec = DateOnly.FromDateTime(DateTime.Today),
                IdCaLamViec = Guid.NewGuid()
            };

            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Ca làm việc không tồn tại hoặc đã bị xoá");
        }

        [Fact]
        public async Task Handle_ValidRequest_UpdateExisting_UpdatesPhanCongCa()
        {
            var date = DateOnly.FromDateTime(DateTime.Today);
            var caId1 = Guid.NewGuid();
            var caId2 = Guid.NewGuid();
            var caLamViec = new CaLamViec { Id = caId1, TenCa = "Ca 1", GioBatDau = new TimeSpan(8, 0, 0), GioKetThuc = new TimeSpan(17, 0, 0) };
            _context.CaLamViecs.Add(caLamViec);

            var phanCong = new PhanCongCa
            {
                CccdNhanVien = "001",
                NgayLamViec = date,
                IdCaLamViec = caId2,
                IsDeleted = true // Previously soft-deleted
            };
            _context.PhanCongCas.Add(phanCong);
            await _context.SaveChangesAsync();

            var command = new UpsertPhanCongCaCommand
            {
                CccdNhanVien = "001",
                NgayLamViec = date,
                IdCaLamViec = caId1,
                GhiChu = "Updated shift"
            };

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            phanCong.IdCaLamViec.Should().Be(caId1);
            phanCong.IsDeleted.Should().BeFalse();
            phanCong.GhiChu.Should().Be("Updated shift");
        }

        [Fact]
        public async Task Handle_ValidRequest_CreateNew_AddsPhanCongCa()
        {
            var date = DateOnly.FromDateTime(DateTime.Today);
            var caId1 = Guid.NewGuid();
            var caLamViec = new CaLamViec { Id = caId1, TenCa = "Ca 1", GioBatDau = new TimeSpan(8, 0, 0), GioKetThuc = new TimeSpan(17, 0, 0) };
            _context.CaLamViecs.Add(caLamViec);
            await _context.SaveChangesAsync();

            var command = new UpsertPhanCongCaCommand
            {
                CccdNhanVien = "002",
                NgayLamViec = date,
                IdCaLamViec = caId1,
                GhiChu = "New shift"
            };

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Succeeded.Should().BeTrue();

            var saved = _context.PhanCongCas.FirstOrDefault(p => p.CccdNhanVien == "002" && p.NgayLamViec == date);
            saved.Should().NotBeNull();
            saved!.IdCaLamViec.Should().Be(caId1);
            saved.GhiChu.Should().Be("New shift");
            saved.IsDeleted.Should().BeFalse();
        }

        [Fact]
        public async Task Handle_ValidRequest_IdCaLamViecNull_UpdatesSuccessfully()
        {
            var date = DateOnly.FromDateTime(DateTime.Today);
            var command = new UpsertPhanCongCaCommand
            {
                CccdNhanVien = "003",
                NgayLamViec = date,
                IdCaLamViec = null, // Assigning to Ngay Nghi (override default shift)
                GhiChu = "Override to off day"
            };

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            result.Message.Should().Contain("Đã gán ngày nghỉ ghi đè ca mặc định thành công");

            var saved = _context.PhanCongCas.FirstOrDefault(p => p.CccdNhanVien == "003" && p.NgayLamViec == date);
            saved.Should().NotBeNull();
            saved!.IdCaLamViec.Should().BeNull();
        }
    }
}
