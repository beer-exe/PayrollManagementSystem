using FluentAssertions;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Features.JobGrades.Commands.DeleteJobGrade;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;

namespace PayrollManagementSystem.UnitTests.Application.Features.JobGrades.Commands.DeleteJobGrade
{
    public class DeleteJobGradeCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly DeleteJobGradeCommandHandler _handler;

        public DeleteJobGradeCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new DeleteJobGradeCommandHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_EntityNotFound_ThrowsApiException()
        {
            var command = new DeleteJobGradeCommand { IdNgachLuong = "INVALID_ID" };
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Không tìm thấy ngạch lương");
        }

        [Fact]
        public async Task Handle_HasPositions_ThrowsApiException()
        {
            _context.NgachLuongs.Add(new NgachLuong { IdNgachLuong = "NL01", TenNgachLuong = "Ngạch 1" });
            _context.ChucVus.Add(new ChucVu { IdChucVu = "CV01", TenChucVu = "Chức vụ 1", IdPhongBan = "PB01", IdNgachLuong = "NL01" });
            await _context.SaveChangesAsync();

            var command = new DeleteJobGradeCommand { IdNgachLuong = "NL01" };
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("đã được gán cho chức vụ");
        }

        [Fact]
        public async Task Handle_ValidRequest_SoftDeletesJobGrade()
        {
            _context.NgachLuongs.Add(new NgachLuong { IdNgachLuong = "NL01", TenNgachLuong = "Ngạch 1" });
            await _context.SaveChangesAsync();

            var command = new DeleteJobGradeCommand { IdNgachLuong = "NL01" };
            var result = await _handler.Handle(command, CancellationToken.None);

            result.Succeeded.Should().BeTrue();

            var entity = await _context.NgachLuongs.FindAsync("NL01");
            entity.Should().NotBeNull();
            entity!.IsDeleted.Should().BeTrue();
        }
    }
}
