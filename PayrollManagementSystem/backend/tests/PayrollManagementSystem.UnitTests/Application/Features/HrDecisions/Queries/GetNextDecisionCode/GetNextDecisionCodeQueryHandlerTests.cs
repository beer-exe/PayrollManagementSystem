using FluentAssertions;
using PayrollManagementSystem.Application.Features.HrDecisions.Queries.GetNextDecisionCode;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.HrDecisions.Queries.GetNextDecisionCode
{
    public class GetNextDecisionCodeQueryHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly GetNextDecisionCodeQueryHandler _handler;

        public GetNextDecisionCodeQueryHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new GetNextDecisionCodeQueryHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_NoExistingDecisions_ReturnsFirstCode()
        {
            // Arrange
            var query = new GetNextDecisionCodeQuery { Type = "TD" };
            var year = DateTime.UtcNow.Year;

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().Be($"QDTD-{year}-000001");
        }

        [Fact]
        public async Task Handle_ExistingDecisions_ReturnsNextCode()
        {
            // Arrange
            var year = DateTime.UtcNow.Year;
            _context.QuyetDinhNhanSus.Add(new QuyetDinhNhanSu { SoQuyetDinh = $"QDTD-{year}-000005", Cccd = "001", LoaiQuyetDinh = "TD", IdChucVuMoi = "CV01" });
            _context.QuyetDinhNhanSus.Add(new QuyetDinhNhanSu { SoQuyetDinh = $"QDTD-{year}-000002", Cccd = "002", LoaiQuyetDinh = "TD", IdChucVuMoi = "CV01" });
            await _context.SaveChangesAsync();

            var query = new GetNextDecisionCodeQuery { Type = "TD" };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().Be($"QDTD-{year}-000006");
        }
        
        [Fact]
        public async Task Handle_ExistingDecisionsDifferentType_ReturnsFirstCodeForNewType()
        {
            // Arrange
            var year = DateTime.UtcNow.Year;
            _context.QuyetDinhNhanSus.Add(new QuyetDinhNhanSu { SoQuyetDinh = $"QDTD-{year}-000005", Cccd = "001", LoaiQuyetDinh = "TD", IdChucVuMoi = "CV01" });
            await _context.SaveChangesAsync();

            var query = new GetNextDecisionCodeQuery { Type = "BN" };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().Be($"QDBN-{year}-000001");
        }
    }
}
