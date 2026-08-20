using FluentAssertions;
using PayrollManagementSystem.Application.Features.Users.Queries.GetEmployeesWithoutAccount;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;

namespace PayrollManagementSystem.UnitTests.Application.Features.Users.Queries.GetEmployeesWithoutAccount
{
    public class GetEmployeesWithoutAccountQueryHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly GetEmployeesWithoutAccountQueryHandler _handler;

        public GetEmployeesWithoutAccountQueryHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new GetEmployeesWithoutAccountQueryHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_ReturnsOnlyEmployeesWithoutAccount()
        {
            var pb = new PhongBan { IdPb = "PB01", TenPb = "Phòng IT" };
            _context.PhongBans.Add(pb);

            // Employee with account
            _context.NhanViens.Add(new NhanVien
            {
                Cccd = "111",
                HoTen = "With Account",
                Sdt = "0111",
                Email = "1@abc.com",
                DiaChi = "HN",
                NgaySinh = DateOnly.FromDateTime(DateTime.Today.AddYears(-20)),
                GioiTinh = true,
                IdPb = "PB01",
                NgayVaoLam = DateOnly.FromDateTime(DateTime.Today),
                TrangThai = TrangThaiNhanVien.DANG_LAM_VIEC,
                HeSoP2 = 1.0m,
                IdTaiKhoan = Guid.NewGuid()
            });

            // Employee without account
            _context.NhanViens.Add(new NhanVien
            {
                Cccd = "222",
                HoTen = "Without Account",
                Sdt = "0222",
                Email = "2@abc.com",
                DiaChi = "HN",
                NgaySinh = DateOnly.FromDateTime(DateTime.Today.AddYears(-20)),
                GioiTinh = false,
                IdPb = "PB01",
                NgayVaoLam = DateOnly.FromDateTime(DateTime.Today),
                TrangThai = TrangThaiNhanVien.DANG_LAM_VIEC,
                HeSoP2 = 1.0m,
                IdTaiKhoan = null
            });
            await _context.SaveChangesAsync();

            var query = new GetEmployeesWithoutAccountQuery();
            var result = await _handler.Handle(query, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            result.Data.Should().HaveCount(1);
            result.Data.First().Cccd.Should().Be("222");
            result.Data.First().HoTen.Should().Be("Without Account");
            result.Data.First().TenPhongBan.Should().Be("Phòng IT");
        }
    }
}
