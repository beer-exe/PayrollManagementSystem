using PayrollManagementSystem.Domain.Common;

namespace PayrollManagementSystem.Domain.Models
{
    public class CaLamViec : BaseAuditableEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string TenCa { get; set; } = null!;
        public TimeSpan GioBatDau { get; set; }
        public TimeSpan GioKetThuc { get; set; }
        public bool XuyenNgay { get; set; }
        public decimal HeSoLuong { get; set; } = 1.0m;
        public bool TrangThai { get; set; } = true;

        // Navigation properties
        public ICollection<KhungGioNghi> KhungGioNghis { get; set; } = new List<KhungGioNghi>();
    }
}
