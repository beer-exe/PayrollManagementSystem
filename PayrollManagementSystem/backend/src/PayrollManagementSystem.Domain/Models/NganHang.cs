namespace PayrollManagementSystem.Domain.Models
{
    public class NganHang
    {
        public Guid IdNganHang { get; set; }
        public string TenNganHang { get; set; } = null!;

        // Navigation properties
        public ICollection<TaiKhoanNganHang> TaiKhoanNganHangs { get; set; } = new List<TaiKhoanNganHang>();
    }
}
