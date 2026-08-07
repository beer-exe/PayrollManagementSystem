using FluentAssertions;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Features.SalarySteps.Commands.CreateSalaryStep;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.SalarySteps.Commands.CreateSalaryStep
{
    public class CreateSalaryStepCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly CreateSalaryStepCommandHandler _handler;

        public CreateSalaryStepCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new CreateSalaryStepCommandHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_StepExists_ThrowsApiException()
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

            var command = new CreateSalaryStepCommand
            {
                JobGradeId = "NL01",
                StepName = "Bậc 1",
                P1Salary = 6000000,
                EffectiveDate = DateTime.Today.AddDays(1)
            };

            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("đã tồn tại");
        }

        [Fact]
        public async Task Handle_ValidRequest_CreatesSalaryStep()
        {
            var command = new CreateSalaryStepCommand
            {
                JobGradeId = "NL01",
                StepName = "Bậc 1",
                P1Salary = 5000000,
                EffectiveDate = DateTime.Today
            };

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            result.Data.Should().NotBeNullOrEmpty();
            
            var saved = await _context.BacLuongs.FindAsync(result.Data);
            saved.Should().NotBeNull();
            saved!.IdNgachLuong.Should().Be("NL01");
            saved.TenBacLuong.Should().Be("Bậc 1");
            saved.LuongP1.Should().Be(5000000);
            saved.TrangThai.Should().Be(TrangThaiBacLuong.HIEU_LUC);
        }
    }
}
