using FluentAssertions;
using PayrollManagementSystem.Application.Features.Departments.Queries.GetEmployeesByDepartment;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;

namespace PayrollManagementSystem.UnitTests.Application.Features.Departments.Queries.GetEmployeesByDepartment
{
    public class GetEmployeesByDepartmentQueryHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly GetEmployeesByDepartmentQueryHandler _handler;

        public GetEmployeesByDepartmentQueryHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new GetEmployeesByDepartmentQueryHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsEmployees()
        {
            // Arrange
            _context.PhongBans.Add(new PhongBan { IdPb = "PB1", TenPb = "Phòng 1" });

            var nv1 = new NhanVien { Cccd = "001", HoTen = "Test 1", IdPb = "PB1" };
            var nv2 = new NhanVien { Cccd = "002", HoTen = "Test 2", IdPb = "PB2" };

            _context.NhanViens.AddRange(nv1, nv2);
            await _context.SaveChangesAsync();

            var query = new GetEmployeesByDepartmentQuery { IdPb = "PB1" };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            result.Data.Should().HaveCount(1);
            result.Data.First().Cccd.Should().Be("001");
        }
    }
}
