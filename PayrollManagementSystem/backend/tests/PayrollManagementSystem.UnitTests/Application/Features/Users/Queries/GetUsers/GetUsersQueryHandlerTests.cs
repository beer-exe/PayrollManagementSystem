using FluentAssertions;
using PayrollManagementSystem.Application.Features.Users.Queries.GetUsers;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Extensions;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.Users.Queries.GetUsers
{
    public class GetUsersQueryHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly GetUsersQueryHandler _handler;

        public GetUsersQueryHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new GetUsersQueryHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_ReturnsUsersWithEmployeeAndRoleIncluded()
        {
            var roleId = Guid.NewGuid();
            var role = new VaiTro { IdVaiTro = roleId, TenVaiTro = "Admin Role" };
            _context.VaiTros.Add(role);

            var nv = new NhanVien
            {
                Cccd = "123",
                HoTen = "Test User",
                Sdt = "0123",
                Email = "test@user.com",
                DiaChi = "HN",
                NgaySinh = DateOnly.FromDateTime(DateTime.Today.AddYears(-20)),
                GioiTinh = true,
                IdPb = "PB01",
                NgayVaoLam = DateOnly.FromDateTime(DateTime.Today),
                TrangThai = TrangThaiNhanVien.DANG_LAM_VIEC,
                HeSoP2 = 1.0m,
            };
            
            var accountId = Guid.NewGuid();
            var account = new TaiKhoan
            {
                IdTaiKhoan = accountId,
                TenTaiKhoan = "admin_user",
                MatKhauHash = "hash",
                IdVaiTro = roleId,
                TrangThai = TrangThaiTaiKhoan.HOAT_DONG,
                VaiTro = role,
                NhanVien = nv
            };
            _context.TaiKhoans.Add(account);
            await _context.SaveChangesAsync();

            var query = new GetUsersQuery();
            var result = await _handler.Handle(query, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            result.Data.Should().HaveCount(1);
            
            var dto = result.Data.First();
            dto.IdTaiKhoan.Should().Be(accountId);
            dto.TenTaiKhoan.Should().Be("admin_user");
            dto.HoTen.Should().Be("Test User");
            dto.Email.Should().Be("test@user.com");
            dto.TenVaiTro.Should().Be("Admin Role");
            dto.TrangThai.Should().Be(TrangThaiTaiKhoan.HOAT_DONG.GetDescription());
        }
    }
}
