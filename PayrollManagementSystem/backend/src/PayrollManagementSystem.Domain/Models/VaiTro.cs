namespace PayrollManagementSystem.Domain.Models
{
    public class VaiTro
    {
        public Guid IdVaiTro { get; set; }
        public string TenVaiTro { get; set; } = null!;

        // Navigation properties
        public ICollection<TaiKhoan> TaiKhoans { get; set; } = new List<TaiKhoan>();
    }
}
