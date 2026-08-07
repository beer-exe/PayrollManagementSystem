using FluentAssertions;
using Moq;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.Users.Commands.CreateUser;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.Users.Commands.CreateUser
{
    public class CreateUserCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly CreateUserCommandHandler _handler;

        public CreateUserCommandHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _handler = new CreateUserCommandHandler(_context, _passwordHasherMock.Object);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_UsernameExists_ThrowsApiException()
        {
            _context.TaiKhoans.Add(new TaiKhoan { TenTaiKhoan = "admin", MatKhauHash = "x" });
            await _context.SaveChangesAsync();

            var command = new CreateUserCommand { TenTaiKhoan = "admin", MatKhau = "123", Cccd = "123", IdVaiTro = Guid.NewGuid() };

            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Tên tài khoản đã tồn tại");
        }

        [Fact]
        public async Task Handle_EmployeeNotFound_ThrowsApiException()
        {
            var command = new CreateUserCommand { TenTaiKhoan = "newuser", MatKhau = "123", Cccd = "123", IdVaiTro = Guid.NewGuid() };

            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Không tìm thấy nhân viên");
        }

        [Fact]
        public async Task Handle_EmployeeAlreadyHasAccount_ThrowsApiException()
        {
            var nv = new NhanVien
            {
                Cccd = "123",
                HoTen = "Test",
                Sdt = "0123",
                Email = "test@abc.com",
                DiaChi = "HN",
                NgaySinh = DateOnly.FromDateTime(DateTime.Today.AddYears(-20)),
                GioiTinh = true,
                IdPb = "PB01",
                NgayVaoLam = DateOnly.FromDateTime(DateTime.Today),
                TrangThai = TrangThaiNhanVien.DANG_LAM_VIEC,
                HeSoP2 = 1.0m,
                IdTaiKhoan = Guid.NewGuid() // Has account
            };
            _context.NhanViens.Add(nv);
            await _context.SaveChangesAsync();

            var command = new CreateUserCommand { TenTaiKhoan = "newuser", MatKhau = "123", Cccd = "123", IdVaiTro = Guid.NewGuid() };

            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Nhân viên này đã có tài khoản");
        }

        [Fact]
        public async Task Handle_RoleNotFound_ThrowsApiException()
        {
            var nv = new NhanVien
            {
                Cccd = "123",
                HoTen = "Test",
                Sdt = "0123",
                Email = "test@abc.com",
                DiaChi = "HN",
                NgaySinh = DateOnly.FromDateTime(DateTime.Today.AddYears(-20)),
                GioiTinh = true,
                IdPb = "PB01",
                NgayVaoLam = DateOnly.FromDateTime(DateTime.Today),
                TrangThai = TrangThaiNhanVien.DANG_LAM_VIEC,
                HeSoP2 = 1.0m,
                IdTaiKhoan = null
            };
            _context.NhanViens.Add(nv);
            await _context.SaveChangesAsync();

            var command = new CreateUserCommand { TenTaiKhoan = "newuser", MatKhau = "123", Cccd = "123", IdVaiTro = Guid.NewGuid() };

            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Vai trò được phân quyền không tồn tại");
        }

        [Fact]
        public async Task Handle_ValidRequest_CreatesUser()
        {
            var nv = new NhanVien
            {
                Cccd = "123",
                HoTen = "Test",
                Sdt = "0123",
                Email = "test@abc.com",
                DiaChi = "HN",
                NgaySinh = DateOnly.FromDateTime(DateTime.Today.AddYears(-20)),
                GioiTinh = true,
                IdPb = "PB01",
                NgayVaoLam = DateOnly.FromDateTime(DateTime.Today),
                TrangThai = TrangThaiNhanVien.DANG_LAM_VIEC,
                HeSoP2 = 1.0m,
                IdTaiKhoan = null
            };
            var roleId = Guid.NewGuid();
            _context.NhanViens.Add(nv);
            _context.VaiTros.Add(new VaiTro { IdVaiTro = roleId, TenVaiTro = "Admin" });
            await _context.SaveChangesAsync();

            _passwordHasherMock.Setup(x => x.HashPasswordEnhanced("123")).Returns("hashed_123");

            var command = new CreateUserCommand { TenTaiKhoan = "newuser", MatKhau = "123", Cccd = "123", IdVaiTro = roleId };

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Succeeded.Should().BeTrue();

            var savedUser = await _context.TaiKhoans.FindAsync(result.Data);
            savedUser.Should().NotBeNull();
            savedUser!.TenTaiKhoan.Should().Be("newuser");
            savedUser.MatKhauHash.Should().Be("hashed_123");
            savedUser.IdVaiTro.Should().Be(roleId);
            
            var savedNv = await _context.NhanViens.FindAsync("123");
            savedNv!.IdTaiKhoan.Should().Be(result.Data);
        }
    }
}
