using FluentAssertions;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Features.SalarySteps.Commands.DeleteSalaryStep;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;

namespace PayrollManagementSystem.UnitTests.Application.Features.SalarySteps.Commands.DeleteSalaryStep
{
    public class DeleteSalaryStepCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly DeleteSalaryStepCommandHandler _handler;

        public DeleteSalaryStepCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new DeleteSalaryStepCommandHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_StepNotFound_ThrowsApiException()
        {
            var command = new DeleteSalaryStepCommand
            {
                JobGradeId = "NL01",
                StepName = "Bậc 1"
            };

            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Không tìm thấy dữ liệu bậc lương");
        }

        [Fact]
        public async Task Handle_StepInUse_ThrowsApiException()
        {
            var bl = new BacLuong
            {
                IdBacLuong = "BL01",
                IdNgachLuong = "NL01",
                TenBacLuong = "Bậc 1",
                LuongP1 = 5000000,
                NgayApDung = DateOnly.FromDateTime(DateTime.Today),
                TrangThai = TrangThaiBacLuong.HIEU_LUC
            };
            _context.BacLuongs.Add(bl);

            var qd = new QuyetDinhNhanSu
            {
                SoQuyetDinh = "QD-01",
                Cccd = "001",
                IdBacLuongMoi = "BL01",
                NgayHieuLuc = DateOnly.FromDateTime(DateTime.Today),
                LoaiQuyetDinh = "TUYEN_DUNG",
                TrangThai = TrangThaiQuyetDinh.HIEU_LUC
            };
            _context.QuyetDinhNhanSus.Add(qd);
            await _context.SaveChangesAsync();

            var command = new DeleteSalaryStepCommand
            {
                JobGradeId = "NL01",
                StepName = "Bậc 1"
            };

            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("hiện đang được sử dụng");
        }

        [Fact]
        public async Task Handle_ValidRequest_DeletesSalaryStep()
        {
            var bl1 = new BacLuong
            {
                IdBacLuong = "BL01",
                IdNgachLuong = "NL01",
                TenBacLuong = "Bậc 1",
                LuongP1 = 5000000,
                NgayApDung = DateOnly.FromDateTime(DateTime.Today.AddDays(-10)),
                TrangThai = TrangThaiBacLuong.HET_HIEU_LUC
            };
            var bl2 = new BacLuong
            {
                IdBacLuong = "BL02",
                IdNgachLuong = "NL01",
                TenBacLuong = "Bậc 1",
                LuongP1 = 6000000,
                NgayApDung = DateOnly.FromDateTime(DateTime.Today),
                TrangThai = TrangThaiBacLuong.HIEU_LUC
            };
            _context.BacLuongs.AddRange(bl1, bl2);
            await _context.SaveChangesAsync();

            var command = new DeleteSalaryStepCommand
            {
                JobGradeId = "NL01",
                StepName = "Bậc 1"
            };

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Succeeded.Should().BeTrue();

            var remaining = _context.BacLuongs.Where(x => !x.IsDeleted).ToList();
            remaining.Should().BeEmpty();
        }
    }
}
