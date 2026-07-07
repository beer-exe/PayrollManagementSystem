using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Domain.Models
{
    public class ChucVu
    {
        public string IdChucVu { get; set; } = null!;
        public string? IdNgachLuong { get; set; }
        public string IdPhongBan { get; set; } = null!;
        public string? IdChucVuQuanLy { get; set; }
        public string TenChucVu { get; set; } = null!;
        public string? MoTaCongViec { get; set; }
        public TrangThaiChucVu TrangThai { get; set; } = TrangThaiChucVu.HOAT_DONG;

        // Navigation properties
        public NgachLuong? NgachLuong { get; set; }
        public PhongBan PhongBan { get; set; } = null!;
        public ChucVu? ChucVuQuanLy { get; set; }
        public ICollection<ChucVu> ChucVuCapDuois { get; set; } = new List<ChucVu>();
        public ICollection<KhungNangLucP2> KhungNangLucs { get; set; } = new List<KhungNangLucP2>();
    }
}
