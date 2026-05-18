namespace PayrollManagementSystem.Domain.Models
{
    public class PhongBan
    {
        public string IdPb { get; set; } = null!;
        public string TenPb { get; set; } = null!;

        // Navigation properties
        public ICollection<NhanVien> NhanViens { get; set; } = new List<NhanVien>();
    }
}
