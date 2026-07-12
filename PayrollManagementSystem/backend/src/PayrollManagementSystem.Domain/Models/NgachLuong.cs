using PayrollManagementSystem.Domain.Common;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Domain.Models
{
    public class NgachLuong : BaseAuditableEntity
    {
        public string IdNgachLuong { get; set; } = null!;
        public string TenNgachLuong { get; set; } = null!;
        public string? MoTa { get; set; }
        public TrangThaiNgachLuong TrangThai { get; set; } = TrangThaiNgachLuong.HOAT_DONG;

        // Navigation properties
        public ICollection<ChucVu> ChucVus { get; set; } = new List<ChucVu>();
        public ICollection<BacLuong> BacLuongs { get; set; } = new List<BacLuong>();
    }
}
