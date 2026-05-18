namespace PayrollManagementSystem.Domain.Models
{
    public class MoiQuanHe
    {
        public Guid IdMqh { get; set; }
        public string TenQuanHe { get; set; } = null!;

        // Navigation properties
        public ICollection<ThanNhanNhanVien> ThanNhanNhanViens { get; set; } = new List<ThanNhanNhanVien>();
    }
}
