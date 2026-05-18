using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Domain.Models
{
    public class NhanVien
    {
        public string Cccd { get; set; } = null!;
        public string HoTen { get; set; } = null!;
        public bool? GioiTinh { get; set; }
        public string? Sdt { get; set; }
        public string? Email { get; set; }
        public DateOnly? NgaySinh { get; set; }
        public string? DanToc { get; set; }
        public string? DiaChi { get; set; }
        public string? ChuyenNganh { get; set; }
        public DateOnly? NgayVaoLam { get; set; }
        public DateOnly? NgayNghiViec { get; set; }
        public TrangThaiNhanVien? TrangThai { get; set; } = TrangThaiNhanVien.DANG_LAM_VIEC;
        public string? SoBhxh { get; set; }
        public string? SoBhyt { get; set; }
        public string? IdPb { get; set; }
        public Guid? IdTaiKhoan { get; set; }

        // Navigation properties
        public PhongBan? PhongBan { get; set; }
        public TaiKhoan? TaiKhoan { get; set; }
        public ICollection<QuyetDinhNhanSu> QuyetDinhNhanSus { get; set; } = new List<QuyetDinhNhanSu>();
        public ICollection<TaiKhoanNganHang> TaiKhoanNganHangs { get; set; } = new List<TaiKhoanNganHang>();
        public ICollection<ThanNhanNhanVien> ThanNhanNhanViens { get; set; } = new List<ThanNhanNhanVien>();
    }
}
