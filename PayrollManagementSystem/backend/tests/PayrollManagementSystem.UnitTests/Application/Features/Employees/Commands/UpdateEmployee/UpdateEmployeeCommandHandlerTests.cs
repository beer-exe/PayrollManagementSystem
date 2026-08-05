using FluentAssertions;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Features.Employees.Commands.UpdateEmployee;
using PayrollManagementSystem.Application.Features.Employees.DTOs;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.Employees.Commands.UpdateEmployee
{
    public class UpdateEmployeeCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly UpdateEmployeeCommandHandler _handler;

        public UpdateEmployeeCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new UpdateEmployeeCommandHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_EmployeeNotFound_ThrowsKeyNotFoundException()
        {
            var command = new UpdateEmployeeCommand { Cccd = "001", HoTen = "Test NV" };
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Không tìm thấy thông tin hồ sơ nhân viên");
        }

        [Fact]
        public async Task Handle_EmailExistsForOtherEmployee_ThrowsApiException()
        {
            _context.NhanViens.Add(new NhanVien { Cccd = "001", HoTen = "Test1", Email = "test1@test.com" });
            _context.NhanViens.Add(new NhanVien { Cccd = "002", HoTen = "Test2", Email = "test2@test.com" });
            await _context.SaveChangesAsync();

            var command = new UpdateEmployeeCommand { Cccd = "001", HoTen = "Test NV", Email = "test2@test.com" };
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Email này đã được sử dụng");
        }

        [Fact]
        public async Task Handle_DepartmentNotFound_ThrowsApiException()
        {
            _context.NhanViens.Add(new NhanVien { Cccd = "001", HoTen = "Test1", Email = "test1@test.com" });
            await _context.SaveChangesAsync();

            var command = new UpdateEmployeeCommand { Cccd = "001", HoTen = "Test NV", IdPb = "INVALID_PB" };
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Phòng ban được chọn không tồn tại");
        }

        [Fact]
        public async Task Handle_ValidRequestWithoutRelations_UpdatesEmployee()
        {
            _context.NhanViens.Add(new NhanVien { Cccd = "001", HoTen = "Old Name" });
            await _context.SaveChangesAsync();

            var command = new UpdateEmployeeCommand { Cccd = "001", HoTen = "New Name" };
            var result = await _handler.Handle(command, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            
            var nv = await _context.NhanViens.FindAsync("001");
            nv!.HoTen.Should().Be("New Name");
        }

        [Fact]
        public async Task Handle_ValidRequestWithRelations_AddsAndUpdatesRelations()
        {
            var guid1 = Guid.NewGuid();
            var guid2 = Guid.NewGuid();
            var guid3 = Guid.NewGuid();

            _context.NhanViens.Add(new NhanVien { Cccd = "001", HoTen = "Test" });
            _context.ThanNhans.Add(new ThanNhan { MaDinhDanh = "TN1", TenTn = "Old TN" });
            _context.TNhanNviens.Add(new ThanNhanNhanVien { Cccd = "001", MaDinhDanh = "TN1", IdMqh = guid1 });
            await _context.SaveChangesAsync();

            var command = new UpdateEmployeeCommand 
            { 
                Cccd = "001", 
                HoTen = "Test",
                ThanNhans = new List<UpdateThanNhanDto>
                {
                    new UpdateThanNhanDto { MaDinhDanh = "TN1", TenTn = "Updated TN", IdMqh = guid2 }, // Update existing
                    new UpdateThanNhanDto { TenTn = "New TN", IdMqh = guid3 } // Add new
                }
            };
            
            var result = await _handler.Handle(command, CancellationToken.None);
            result.Succeeded.Should().BeTrue();

            // Verify existing is updated
            var tn1 = await _context.ThanNhans.FindAsync("TN1");
            tn1!.TenTn.Should().Be("Updated TN");

            // Verify new relation is added
            var relations = _context.TNhanNviens.Where(t => t.Cccd == "001").ToList();
            relations.Count.Should().Be(2);
            var newRelation = relations.First(r => r.MaDinhDanh != "TN1");
            newRelation.IdMqh.Should().Be(guid3);
        }
    }
}
