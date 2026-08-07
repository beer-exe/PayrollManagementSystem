using FluentAssertions;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Features.Profile.Queries.GetUserProfile;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.UnitTests.Mocks;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.Profile.Queries.GetUserProfile
{
    public class GetUserProfileQueryHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly GetUserProfileQueryHandler _handler;

        public GetUserProfileQueryHandlerTests()
        {
            _context = MockDbContextFactory.Create();
            _handler = new GetUserProfileQueryHandler(_context);
        }

        public void Dispose()
        {
            MockDbContextFactory.Destroy(_context);
        }

        [Fact]
        public async Task Handle_NhanVienNotFound_ThrowsApiException()
        {
            var query = new GetUserProfileQuery { TaiKhoanId = Guid.NewGuid() };
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(query, CancellationToken.None));
            exception.Message.Should().Contain("Không tìm thấy thông tin hồ sơ");
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsProfile()
        {
            // Arrange
            var taiKhoanId = Guid.NewGuid();
            var cccd = "001";
            
            _context.TaiKhoans.Add(new TaiKhoan { IdTaiKhoan = taiKhoanId, TenTaiKhoan = "test", MatKhauHash = "hash", UserAvatar = "avatar.jpg" });
            
            var pb = new PhongBan { IdPb = "PB01", TenPb = "IT" };
            _context.PhongBans.Add(pb);
            
            var cv = new ChucVu { IdChucVu = "CV01", TenChucVu = "Dev", IdPhongBan = "PB01" };
            _context.ChucVus.Add(cv);

            _context.NhanViens.Add(new NhanVien
            {
                Cccd = cccd,
                HoTen = "Nguyen Van A",
                IdTaiKhoan = taiKhoanId,
                IdPb = "PB01",
                PhongBan = pb,
                Email = "test@example.com"
            });
            
            _context.QuyetDinhNhanSus.Add(new QuyetDinhNhanSu
            {
                SoQuyetDinh = "QD-01",
                Cccd = cccd,
                IdChucVuMoi = "CV01",
                TrangThai = TrangThaiQuyetDinh.HIEU_LUC,
                NgayHieuLuc = DateOnly.FromDateTime(DateTime.Today.AddDays(-10)),
                LoaiQuyetDinh = "TUYEN_DUNG"
            });
            
            await _context.SaveChangesAsync();

            var query = new GetUserProfileQuery { TaiKhoanId = taiKhoanId };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Cccd.Should().Be(cccd);
            result.Data.HoTen.Should().Be("Nguyen Van A");
            result.Data.Email.Should().Be("test@example.com");
            result.Data.TenPhongBan.Should().Be("IT");
            result.Data.TenChucVu.Should().Be("Dev");
            result.Data.UserAvatar.Should().Be("avatar.jpg");
        }
    }
}
