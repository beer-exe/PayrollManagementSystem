using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Domain.Models
{
    public class ChucVu
    {
        public string IdChucVu { get; set; } = null!;
        public string TenChucVu { get; set; } = null!;
        public string? MoTaCongViec { get; set; }
        public TrangThaiChucVu TrangThai { get; set; } = TrangThaiChucVu.HOAT_DONG;

        // Navigation properties
        public ICollection<BacLuong> BacLuongs { get; set; } = new List<BacLuong>();
        public ICollection<KhungNangLucP2> KhungNangLucs { get; set; } = new List<KhungNangLucP2>();

    }
}
