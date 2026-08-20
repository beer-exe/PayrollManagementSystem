using FluentAssertions;
using PayrollManagementSystem.Application.Features.JobGrades.Commands.CreateJobGrade;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;

namespace PayrollManagementSystem.UnitTests.Application.Features.JobGrades.Commands.CreateJobGrade
{
    public class CreateJobGradeCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly CreateJobGradeCommandHandler _handler;

        public CreateJobGradeCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new CreateJobGradeCommandHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_ValidRequest_CreatesJobGrade()
        {
            var command = new CreateJobGradeCommand
            {
                TenNgachLuong = "Ngạch Chuyên Viên",
                MoTa = "Ngạch dành cho chuyên viên"
            };

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            result.Message.Should().Contain("thành công");

            var entity = await _context.NgachLuongs.FindAsync(result.Data);
            entity.Should().NotBeNull();
            entity!.TenNgachLuong.Should().Be("Ngạch Chuyên Viên");
            entity.MoTa.Should().Be("Ngạch dành cho chuyên viên");
            entity.TrangThai.Should().Be(TrangThaiNgachLuong.HOAT_DONG);
        }
    }
}
