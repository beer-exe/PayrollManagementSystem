namespace PayrollManagementSystem.Domain.Models
{
    public class ChucVu
    {
        public string IdChucVu { get; set; } = null!;
        public string TenChucVu { get; set; } = null!;

        // Navigation properties
        public ICollection<BacLuong> BacLuongs { get; set; } = new List<BacLuong>();
    }
}
