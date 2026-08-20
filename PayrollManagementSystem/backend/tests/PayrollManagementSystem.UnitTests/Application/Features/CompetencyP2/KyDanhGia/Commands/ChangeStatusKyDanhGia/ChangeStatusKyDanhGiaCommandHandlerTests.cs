using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Features.CompetencyP2.KyDanhGia.Commands.ChangeStatusKyDanhGia;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;

namespace PayrollManagementSystem.UnitTests.Application.Features.CompetencyP2.KyDanhGia.Commands.ChangeStatusKyDanhGia
{
    public class ChangeStatusKyDanhGiaCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly ChangeStatusKyDanhGiaCommandHandler _handler;

        public ChangeStatusKyDanhGiaCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new ChangeStatusKyDanhGiaCommandHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_OpenEvaluationFromInitial_UpdatesStatus()
        {
            // Arrange
            var kyDanhGia = new Domain.Models.KyDanhGia { IdKyDanhGia = Guid.NewGuid(), TenKyDanhGia = "Test", TrangThai = TrangThaiKyDanhGia.KHOI_TAO };
            _context.KyDanhGias.Add(kyDanhGia);
            await _context.SaveChangesAsync();

            var command = new ChangeStatusKyDanhGiaCommand { IdKyDanhGia = kyDanhGia.IdKyDanhGia, TrangThaiMoi = TrangThaiKyDanhGia.DANG_DANH_GIA };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            var dbEntity = await _context.KyDanhGias.FindAsync(kyDanhGia.IdKyDanhGia);
            dbEntity!.TrangThai.Should().Be(TrangThaiKyDanhGia.DANG_DANH_GIA);
        }

        [Fact]
        public async Task Handle_CloseEvaluation_UpdatesStatusAndCalculatesScores()
        {
            // Arrange
            var kyDanhGia = new Domain.Models.KyDanhGia { IdKyDanhGia = Guid.NewGuid(), TenKyDanhGia = "Test", TrangThai = TrangThaiKyDanhGia.DANG_DANH_GIA };
            var nv = new NhanVien { Cccd = "001", HoTen = "Test NV", HeSoP2 = 0 };
            var phieu = new PhieuDanhGiaNangLuc { IdPhieu = Guid.NewGuid(), IdKyDanhGia = kyDanhGia.IdKyDanhGia, CccdNhanVien = "001", TrangThai = TrangThaiPhieuDanhGia.DA_HOAN_THANH, DiemTongHop = 80 };
            var config = new MucQuyDoiP2 { IdQuyDoi = Guid.NewGuid(), DiemToiThieu = 70, DiemToiDa = 100, HeSoP2 = 1.2m, XepLoai = "Xuất sắc" };

            _context.KyDanhGias.Add(kyDanhGia);
            _context.NhanViens.Add(nv);
            _context.PhieuDanhGiaNangLucs.Add(phieu);
            _context.MucQuyDoiP2s.Add(config);
            await _context.SaveChangesAsync();

            var command = new ChangeStatusKyDanhGiaCommand { IdKyDanhGia = kyDanhGia.IdKyDanhGia, TrangThaiMoi = TrangThaiKyDanhGia.DA_CHOT };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();

            var dbKy = await _context.KyDanhGias.FindAsync(kyDanhGia.IdKyDanhGia);
            dbKy!.TrangThai.Should().Be(TrangThaiKyDanhGia.DA_CHOT);

            var dbPhieu = await _context.PhieuDanhGiaNangLucs.FindAsync(phieu.IdPhieu);
            dbPhieu!.HeSoP2.Should().Be(1.2m);
            dbPhieu.XepLoai.Should().Be("Xuất sắc");

            // Verify that employee was updated
            var dbNvByCccd = await _context.NhanViens.FirstOrDefaultAsync(x => x.Cccd == "001");
            dbNvByCccd!.HeSoP2.Should().Be(1.2m);
        }

        [Fact]
        public async Task Handle_CancelEvaluation_CancelsAllPhieu()
        {
            // Arrange
            var kyDanhGia = new Domain.Models.KyDanhGia { IdKyDanhGia = Guid.NewGuid(), TenKyDanhGia = "Test", TrangThai = TrangThaiKyDanhGia.DANG_DANH_GIA };
            var phieu = new PhieuDanhGiaNangLuc { IdPhieu = Guid.NewGuid(), IdKyDanhGia = kyDanhGia.IdKyDanhGia, CccdNhanVien = "001", TrangThai = TrangThaiPhieuDanhGia.CHO_NV_DANH_GIA };

            _context.KyDanhGias.Add(kyDanhGia);
            _context.PhieuDanhGiaNangLucs.Add(phieu);
            await _context.SaveChangesAsync();

            var command = new ChangeStatusKyDanhGiaCommand { IdKyDanhGia = kyDanhGia.IdKyDanhGia, TrangThaiMoi = TrangThaiKyDanhGia.DA_HUY };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            var dbPhieu = await _context.PhieuDanhGiaNangLucs.FindAsync(phieu.IdPhieu);
            dbPhieu!.TrangThai.Should().Be(TrangThaiPhieuDanhGia.DA_HUY);
        }
    }
}
