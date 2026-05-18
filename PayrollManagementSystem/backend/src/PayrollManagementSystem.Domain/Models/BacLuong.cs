namespace PayrollManagementSystem.Domain.Models
{
    public class BacLuong
    {
        public string IdBacLuong { get; set; } = null!;
        public string IdChucVu { get; set; } = null!;
        public decimal LuongP1 { get; set; }
        public DateOnly NgayApDung { get; set; }

        // Navigation properties
        public ChucVu ChucVu { get; set; } = null!;
        public ICollection<QuyetDinhNhanSu> QuyetDinhNhanSus { get; set; } = new List<QuyetDinhNhanSu>();
    }
}
