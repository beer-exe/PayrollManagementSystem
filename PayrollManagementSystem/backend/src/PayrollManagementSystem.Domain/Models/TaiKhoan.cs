using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Domain.Models
{
    public class TaiKhoan
    {
        public Guid IdTaiKhoan { get; set; }
        public string TenTaiKhoan { get; set; } = null!;
        public string MatKhauHash { get; set; } = null!;
        public TrangThaiTaiKhoan TrangThai { get; set; } = TrangThaiTaiKhoan.HOAT_DONG;
        public bool? DangNhapLanDau { get; set; } = true;
        public Guid? IdVaiTro { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        // Navigation properties
        public NhanVien? NhanVien { get; set; }
        public VaiTro? VaiTro { get; set; }
    }
}
