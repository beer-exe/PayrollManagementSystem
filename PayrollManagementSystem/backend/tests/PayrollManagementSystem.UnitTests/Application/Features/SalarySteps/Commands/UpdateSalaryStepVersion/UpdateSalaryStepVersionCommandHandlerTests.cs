using FluentAssertions;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Features.SalarySteps.Commands.UpdateSalaryStepVersion;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.SalarySteps.Commands.UpdateSalaryStepVersion
{
    public class UpdateSalaryStepVersionCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly UpdateSalaryStepVersionCommandHandler _handler;

        public UpdateSalaryStepVersionCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new UpdateSalaryStepVersionCommandHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_ActiveVersionNotFound_ThrowsApiException()
        {
            var command = new UpdateSalaryStepVersionCommand
            {
                JobGradeId = "NL01",
                StepName = "Bậc 1",
                NewP1Salary = 6000000,
                NewEffectiveDate = DateTime.Today.AddDays(1)
            };

            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Không tìm thấy dữ liệu hiện hành");
        }

        [Fact]
        public async Task Handle_NewDateNotGreaterThanCurrent_ThrowsApiException()
        {
            _context.BacLuongs.Add(new BacLuong
            {
                IdBacLuong = "BL01",
                IdNgachLuong = "NL01",
                TenBacLuong = "Bậc 1",
                LuongP1 = 5000000,
                NgayApDung = DateOnly.FromDateTime(DateTime.Today),
                TrangThai = TrangThaiBacLuong.HIEU_LUC
            });
            await _context.SaveChangesAsync();

            var command = new UpdateSalaryStepVersionCommand
            {
                JobGradeId = "NL01",
                StepName = "Bậc 1",
                NewP1Salary = 6000000,
                NewEffectiveDate = DateTime.Today // Not greater
            };

            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Ngày áp dụng mới phải lớn hơn ngày hiện hành");
        }

        [Fact]
        public async Task Handle_ValidRequest_UpdatesVersion()
        {
            var oldDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-10));
            var currentStep = new BacLuong
            {
                IdBacLuong = "BL01",
                IdNgachLuong = "NL01",
                TenBacLuong = "Bậc 1",
                LuongP1 = 5000000,
                NgayApDung = oldDate,
                TrangThai = TrangThaiBacLuong.HIEU_LUC
            };
            _context.BacLuongs.Add(currentStep);
            await _context.SaveChangesAsync();

            var newDate = DateTime.Today.AddDays(1);
            var command = new UpdateSalaryStepVersionCommand
            {
                JobGradeId = "NL01",
                StepName = "Bậc 1",
                NewP1Salary = 6000000,
                NewEffectiveDate = newDate
            };

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            
            // Assert old is inactive
            currentStep.TrangThai.Should().Be(TrangThaiBacLuong.HET_HIEU_LUC);
            currentStep.NgayKetThuc.Should().Be(DateOnly.FromDateTime(newDate.AddDays(-1)));

            // Assert new is created
            var newStep = await _context.BacLuongs.FindAsync(result.Data);
            newStep.Should().NotBeNull();
            newStep!.LuongP1.Should().Be(6000000);
            newStep.NgayApDung.Should().Be(DateOnly.FromDateTime(newDate));
            newStep.TrangThai.Should().Be(TrangThaiBacLuong.HIEU_LUC);
        }
    }
}
