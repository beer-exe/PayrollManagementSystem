using FluentAssertions;
using PayrollManagementSystem.Application.Features.Employees.Queries.GetEmployees;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.Employees.Queries.GetEmployees
{
    public class GetEmployeesQueryHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly GetEmployeesQueryHandler _handler;

        public GetEmployeesQueryHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new GetEmployeesQueryHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_ReturnsPagedEmployees()
        {
            // Arrange
            _context.PhongBans.Add(new PhongBan { IdPb = "PB01", TenPb = "Phòng IT" });
            _context.NhanViens.Add(new NhanVien { Cccd = "001", HoTen = "Nguyen Van A", IdPb = "PB01", TrangThai = TrangThaiNhanVien.DANG_LAM_VIEC });
            _context.NhanViens.Add(new NhanVien { Cccd = "002", HoTen = "Tran Thi B", IdPb = "PB01", TrangThai = TrangThaiNhanVien.DA_NGHI_VIEC });
            await _context.SaveChangesAsync();

            var query = new GetEmployeesQuery { PageNumber = 1, PageSize = 10 };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            result.Data.Should().HaveCount(2);
            result.Data.Should().Contain(e => e.HoTen == "Nguyen Van A");
            result.Data.Should().Contain(e => e.HoTen == "Tran Thi B");
        }

        [Fact]
        public async Task Handle_WithSearchTerm_ReturnsFilteredEmployees()
        {
            // Arrange
            _context.NhanViens.Add(new NhanVien { Cccd = "001", HoTen = "Nguyen Van A", TrangThai = TrangThaiNhanVien.DANG_LAM_VIEC });
            _context.NhanViens.Add(new NhanVien { Cccd = "002", HoTen = "Tran Thi B", TrangThai = TrangThaiNhanVien.DANG_LAM_VIEC });
            await _context.SaveChangesAsync();

            var query = new GetEmployeesQuery { PageNumber = 1, PageSize = 10, SearchTerm = "Nguyen" };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            result.Data.Should().HaveCount(1);
            result.Data.First().HoTen.Should().Be("Nguyen Van A");
        }
    }
}
