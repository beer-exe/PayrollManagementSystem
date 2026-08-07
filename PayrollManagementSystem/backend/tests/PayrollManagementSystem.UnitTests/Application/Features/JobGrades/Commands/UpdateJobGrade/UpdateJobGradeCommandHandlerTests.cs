using FluentAssertions;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Features.JobGrades.Commands.UpdateJobGrade;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.JobGrades.Commands.UpdateJobGrade
{
    public class UpdateJobGradeCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly UpdateJobGradeCommandHandler _handler;

        public UpdateJobGradeCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new UpdateJobGradeCommandHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_EntityNotFound_ThrowsApiException()
        {
            var command = new UpdateJobGradeCommand { IdNgachLuong = "INVALID_ID" };
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Không tìm thấy ngạch lương");
        }

        [Fact]
        public async Task Handle_ValidRequest_UpdatesJobGrade()
        {
            var jobGrade = new NgachLuong
            {
                IdNgachLuong = "NL01",
                TenNgachLuong = "Ngạch 1",
                MoTa = "Mô tả 1",
                TrangThai = TrangThaiNgachLuong.HOAT_DONG
            };
            _context.NgachLuongs.Add(jobGrade);
            await _context.SaveChangesAsync();

            var command = new UpdateJobGradeCommand
            {
                IdNgachLuong = "NL01",
                TenNgachLuong = "Ngạch 1 Updated",
                MoTa = "Mô tả 1 Updated",
                TrangThai = (int)TrangThaiNgachLuong.NGUNG_HOAT_DONG
            };

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            
            var entity = await _context.NgachLuongs.FindAsync("NL01");
            entity!.TenNgachLuong.Should().Be("Ngạch 1 Updated");
            entity.MoTa.Should().Be("Mô tả 1 Updated");
            entity.TrangThai.Should().Be(TrangThaiNgachLuong.NGUNG_HOAT_DONG);
        }
    }
}
