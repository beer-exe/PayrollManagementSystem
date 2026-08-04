using FluentAssertions;
using PayrollManagementSystem.Application.Features.ChamCong.Commands.GenerateMockChamCong;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using System.Text;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.ChamCong.Commands.GenerateMockChamCong
{
    public class GenerateMockChamCongCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly GenerateMockChamCongCommandHandler _handler;

        public GenerateMockChamCongCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new GenerateMockChamCongCommandHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_ValidRequest_GeneratesCsvFile()
        {
            // Arrange
            var nv = new Domain.Models.NhanVien { Cccd = "001", HoTen = "Test NV", TrangThai = TrangThaiNhanVien.DANG_LAM_VIEC };
            _context.NhanViens.Add(nv);
            await _context.SaveChangesAsync();

            var command = new GenerateMockChamCongCommand { Thang = 1, Nam = 2025 };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.FileName.Should().Be("Mock_ChamCong_01_2025.csv");
            result.ContentType.Should().Be("text/csv");
            result.Data.Should().NotBeNullOrEmpty();
            
            // Validate preamble and content
            var preamble = Encoding.UTF8.GetPreamble();
            var dataWithoutPreamble = new byte[result.Data.Length - preamble.Length];
            Buffer.BlockCopy(result.Data, preamble.Length, dataWithoutPreamble, 0, dataWithoutPreamble.Length);
            var content = Encoding.UTF8.GetString(dataWithoutPreamble);

            content.Should().Contain("CCCD,NgayChamCong,GioVao,GioRa,GhiChu");
            content.Should().Contain("001");
        }
    }
}
