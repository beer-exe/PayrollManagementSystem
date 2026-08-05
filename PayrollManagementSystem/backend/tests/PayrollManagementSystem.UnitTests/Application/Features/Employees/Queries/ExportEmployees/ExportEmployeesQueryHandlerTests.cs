using FluentAssertions;
using Moq;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.Employees.DTOs;
using PayrollManagementSystem.Application.Features.Employees.Queries.ExportEmployees;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.Employees.Queries.ExportEmployees
{
    public class ExportEmployeesQueryHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<IExcelService> _mockExcelService;
        private readonly ExportEmployeesQueryHandler _handler;

        public ExportEmployeesQueryHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _mockExcelService = new Mock<IExcelService>();
            
            _mockExcelService.Setup(x => x.ExportEmployeesToExcel(It.IsAny<List<EmployeeDto>>()))
                .Returns(new byte[] { 1, 2, 3 });

            _handler = new ExportEmployeesQueryHandler(_context, _mockExcelService.Object);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_ReturnsExcelByteArray()
        {
            // Arrange
            _context.NhanViens.Add(new NhanVien { Cccd = "001", HoTen = "Nguyen Van A", TrangThai = TrangThaiNhanVien.DANG_LAM_VIEC });
            await _context.SaveChangesAsync();

            var query = new ExportEmployeesQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(new byte[] { 1, 2, 3 });
            _mockExcelService.Verify(x => x.ExportEmployeesToExcel(It.IsAny<List<EmployeeDto>>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithSearchTermAndDepartmentId_FiltersEmployees()
        {
            // Arrange
            _context.PhongBans.Add(new PhongBan { IdPb = "PB01", TenPb = "Phòng IT" });
            _context.NhanViens.Add(new NhanVien { Cccd = "001", HoTen = "Nguyen Van A", IdPb = "PB01" });
            _context.NhanViens.Add(new NhanVien { Cccd = "002", HoTen = "Tran Thi B", IdPb = "PB02" });
            await _context.SaveChangesAsync();

            var query = new ExportEmployeesQuery { SearchTerm = "Nguyen", IdPb = "PB01" };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            _mockExcelService.Verify(x => x.ExportEmployeesToExcel(It.Is<List<EmployeeDto>>(list => list.Count == 1 && list[0].HoTen == "Nguyen Van A")), Times.Once);
        }
    }
}
