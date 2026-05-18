namespace PayrollManagementSystem.Domain.Models
{
    public class TaiKhoanNganHang
    {
        public string Stk { get; set; } = null!;
        public string? ChiNhanh { get; set; }
        public DateOnly? NgayMoThe { get; set; }
        public string? TrangThai { get; set; }
        public Guid? IdNganHang { get; set; }
        public string? Cccd { get; set; }

        // Navigation properties
        public NhanVien? NhanVien { get; set; }
        public NganHang? NganHang { get; set; }
    }
}
