using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.ChamCong.Commands.ImportChamCong;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using System.Text;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.ChamCong.Commands.ImportChamCong
{
    public class ImportChamCongCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<ITimekeepingCalculatorService> _calculatorServiceMock;
        private readonly ImportChamCongCommandHandler _handler;

        public ImportChamCongCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _calculatorServiceMock = new Mock<ITimekeepingCalculatorService>();
            _handler = new ImportChamCongCommandHandler(_context, _calculatorServiceMock.Object);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_InvalidExtension_ThrowsApiException()
        {
            // Arrange
            var command = new ImportChamCongCommand { FileName = "test.txt", FileStream = new MemoryStream(new byte[10]) };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Chỉ hỗ trợ file định dạng CSV");
        }

        [Fact]
        public async Task Handle_EmptyFile_ThrowsApiException()
        {
            // Arrange
            var command = new ImportChamCongCommand { FileName = "test.csv", FileStream = new MemoryStream() };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("File không hợp lệ hoặc rỗng");
        }

        [Fact]
        public async Task Handle_ValidCsv_ImportsSuccessfully()
        {
            // Arrange
            var nv = new NhanVien { Cccd = "001", HoTen = "Test NV" };
            _context.NhanViens.Add(nv);
            await _context.SaveChangesAsync();

            var csvContent = "CCCD,NgayChamCong,GioVao,GioRa,GhiChu\n" +
                             $"001,{DateTime.Today.AddDays(-1):dd/MM/yyyy},08:00,17:00,Note1\n" +
                             $"001,{DateTime.Today:dd/MM/yyyy},08:15,17:00,Note2";
                             
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

            var command = new ImportChamCongCommand { FileName = "test.csv", FileStream = stream };

            var calcResult = new PayrollManagementSystem.Application.Common.Interfaces.TimekeepingResult
            {
                SoGioLamThucTe = 8,
                SoNgayCong = 1.0m,
                LoaiNgayCong = LoaiNgayCong.LAM_DU_CA,
                GhiChu = "Mocked result"
            };

            _calculatorServiceMock.Setup(x => x.CalculateTimekeepingAsync(It.IsAny<string>(), It.IsAny<DateOnly>(), It.IsAny<TimeOnly?>(), It.IsAny<TimeOnly?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(calcResult);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.TongSoDong.Should().Be(2);
            result.Data.ThanhCong.Should().Be(2);
            result.Data.ThatBai.Should().Be(0);

            var chamCongs = await _context.ChamCongs.ToListAsync();
            chamCongs.Should().HaveCount(2);
            chamCongs.All(c => c.CccdNhanVien == "001").Should().BeTrue();
        }
    }
}
