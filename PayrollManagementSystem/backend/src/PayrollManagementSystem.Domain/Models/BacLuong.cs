using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Domain.Models
{
    public class BacLuong
    {
        public string IdBacLuong { get; set; } = null!;
        public string IdNgachLuong { get; set; } = null!;
        public string TenBacLuong { get; set; } = null!;
        public decimal LuongP1 { get; set; }
        public DateOnly NgayApDung { get; set; }
        public DateOnly? NgayKetThuc { get; set; }
        public TrangThaiBacLuong TrangThai { get; set; } = TrangThaiBacLuong.HIEU_LUC;

        // Navigation properties
        public NgachLuong NgachLuong { get; set; } = null!;
        public ICollection<QuyetDinhNhanSu> QuyetDinhNhanSus { get; set; } = new List<QuyetDinhNhanSu>();
    }
}
