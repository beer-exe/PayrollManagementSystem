using FluentAssertions;
using Moq;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.ChamCong.Commands.CreateChamCong;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.ChamCong.Commands.CreateChamCong
{
    public class CreateChamCongCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<ITimekeepingCalculatorService> _calculatorServiceMock;
        private readonly CreateChamCongCommandHandler _handler;

        public CreateChamCongCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _calculatorServiceMock = new Mock<ITimekeepingCalculatorService>();
            _handler = new CreateChamCongCommandHandler(_context, _calculatorServiceMock.Object);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_EmployeeNotFound_ThrowsApiException()
        {
            // Arrange
            var command = new CreateChamCongCommand { CccdNhanVien = "non-existent" };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Không tìm thấy nhân viên");
        }

        [Fact]
        public async Task Handle_AttendanceAlreadyExists_ThrowsApiException()
        {
            // Arrange
            var nv = new NhanVien { Cccd = "001", HoTen = "Test NV" };
            var chamCong = new Domain.Models.ChamCong { Id = Guid.NewGuid(), CccdNhanVien = "001", NgayChamCong = new DateOnly(2025, 1, 1) };
            
            _context.NhanViens.Add(nv);
            _context.ChamCongs.Add(chamCong);
            await _context.SaveChangesAsync();

            var command = new CreateChamCongCommand { CccdNhanVien = "001", NgayChamCong = new DateOnly(2025, 1, 1) };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Đã tồn tại bản ghi chấm công");
        }

        [Fact]
        public async Task Handle_ValidRequest_CalculatesAndSavesAttendance()
        {
            // Arrange
            var nv = new NhanVien { Cccd = "001", HoTen = "Test NV" };
            _context.NhanViens.Add(nv);
            await _context.SaveChangesAsync();

            var calcResult = new PayrollManagementSystem.Application.Common.Interfaces.TimekeepingResult
            {
                SoGioLamThucTe = 8,
                SoNgayCong = 1.0m,
                LoaiNgayCong = LoaiNgayCong.LAM_DU_CA,
                SoPhutDiTre = 0,
                SoPhutVeSom = 0,
                GhiChu = "Đúng giờ"
            };

            var command = new CreateChamCongCommand
            {
                CccdNhanVien = "001",
                NgayChamCong = new DateOnly(2025, 1, 2),
                GioVao = new TimeOnly(8, 0, 0),
                GioRa = new TimeOnly(17, 0, 0),
                GhiChu = "Test note"
            };

            _calculatorServiceMock.Setup(x => x.CalculateTimekeepingAsync("001", command.NgayChamCong, command.GioVao, command.GioRa, It.IsAny<CancellationToken>()))
                .ReturnsAsync(calcResult);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            result.Data.Should().NotBeEmpty();

            var savedEntity = await _context.ChamCongs.FindAsync(result.Data);
            savedEntity.Should().NotBeNull();
            savedEntity!.SoNgayCong.Should().Be(1.0m);
            savedEntity.GhiChu.Should().Be("Test note");
            savedEntity.TrangThai.Should().Be(TrangThaiChamCong.DA_XAC_NHAN);
        }
    }
}
