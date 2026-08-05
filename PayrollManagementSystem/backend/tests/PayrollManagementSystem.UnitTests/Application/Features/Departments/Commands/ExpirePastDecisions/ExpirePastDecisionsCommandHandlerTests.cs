using FluentAssertions;
using PayrollManagementSystem.Application.Features.Departments.Commands.ExpirePastDecisions;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.Departments.Commands.ExpirePastDecisions
{
    public class ExpirePastDecisionsCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly ExpirePastDecisionsCommandHandler _handler;

        public ExpirePastDecisionsCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new ExpirePastDecisionsCommandHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_MultipleActiveDecisions_ExpiresOlderOnes()
        {
            // Arrange
            var today = DateOnly.FromDateTime(DateTime.Today);
            var yesterday = today.AddDays(-1);
            var lastMonth = today.AddDays(-30);

            var oldQd = new QuyetDinhNhanSu { SoQuyetDinh = "QD1", Cccd = "001", LoaiQuyetDinh = "Test", TrangThai = TrangThaiQuyetDinh.HIEU_LUC, NgayHieuLuc = lastMonth };
            var newQd = new QuyetDinhNhanSu { SoQuyetDinh = "QD2", Cccd = "001", LoaiQuyetDinh = "Test", TrangThai = TrangThaiQuyetDinh.HIEU_LUC, NgayHieuLuc = yesterday };
            
            _context.QuyetDinhNhanSus.AddRange(oldQd, newQd);
            await _context.SaveChangesAsync();

            var command = new ExpirePastDecisionsCommand();

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();

            var dbOldQd = await _context.QuyetDinhNhanSus.FindAsync("QD1");
            dbOldQd!.TrangThai.Should().Be(TrangThaiQuyetDinh.HET_HAN);
            dbOldQd.NgayHetHan.Should().Be(yesterday); // Expired when newQd took effect

            var dbNewQd = await _context.QuyetDinhNhanSus.FindAsync("QD2");
            dbNewQd!.TrangThai.Should().Be(TrangThaiQuyetDinh.HIEU_LUC); // Should remain active
        }
    }
}
