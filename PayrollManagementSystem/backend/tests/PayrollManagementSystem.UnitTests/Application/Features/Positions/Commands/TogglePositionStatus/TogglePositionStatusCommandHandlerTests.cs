using FluentAssertions;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Features.Positions.Commands.TogglePositionStatus;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.Positions.Commands.TogglePositionStatus
{
    public class TogglePositionStatusCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly TogglePositionStatusCommandHandler _handler;

        public TogglePositionStatusCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new TogglePositionStatusCommandHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_NotFound_ThrowsApiException()
        {
            var command = new TogglePositionStatusCommand { IdChucVu = "NON_EXIST" };
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("không tồn tại");
        }

        [Fact]
        public async Task Handle_ActiveWithEmployees_ThrowsApiException()
        {
            _context.ChucVus.Add(new ChucVu { IdChucVu = "CV01", TenChucVu = "Dev", IdPhongBan = "PB01", TrangThai = TrangThaiChucVu.HOAT_DONG });
            _context.QuyetDinhNhanSus.Add(new QuyetDinhNhanSu 
            { 
                SoQuyetDinh = "QD-01", 
                IdChucVuMoi = "CV01", 
                TrangThai = TrangThaiQuyetDinh.HIEU_LUC,
                LoaiQuyetDinh = "TUYEN_DUNG"
            });
            await _context.SaveChangesAsync();

            var command = new TogglePositionStatusCommand { IdChucVu = "CV01" };
            
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("đang được gắn với nhân sự");
        }

        [Fact]
        public async Task Handle_ActiveNoEmployees_TogglesToInactive()
        {
            _context.ChucVus.Add(new ChucVu { IdChucVu = "CV02", TenChucVu = "Dev", IdPhongBan = "PB01", TrangThai = TrangThaiChucVu.HOAT_DONG });
            await _context.SaveChangesAsync();

            var command = new TogglePositionStatusCommand { IdChucVu = "CV02" };
            var result = await _handler.Handle(command, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            var entity = await _context.ChucVus.FindAsync("CV02");
            entity!.TrangThai.Should().Be(TrangThaiChucVu.NGUNG_HOAT_DONG);
        }

        [Fact]
        public async Task Handle_Inactive_TogglesToActive()
        {
            _context.ChucVus.Add(new ChucVu { IdChucVu = "CV03", TenChucVu = "Dev", IdPhongBan = "PB01", TrangThai = TrangThaiChucVu.NGUNG_HOAT_DONG });
            await _context.SaveChangesAsync();

            var command = new TogglePositionStatusCommand { IdChucVu = "CV03" };
            var result = await _handler.Handle(command, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            var entity = await _context.ChucVus.FindAsync("CV03");
            entity!.TrangThai.Should().Be(TrangThaiChucVu.HOAT_DONG);
        }
    }
}
