using PayrollManagementSystem.Domain.Common;
namespace PayrollManagementSystem.Domain.Models
{
    public class ThanNhan : BaseAuditableEntity
    {
        public string MaDinhDanh { get; set; } = null!;
        public string TenTn { get; set; } = null!;
        public DateOnly? NgaySinh { get; set; }

        // Navigation properties
        public ICollection<ThanNhanNhanVien> ThanNhanNhanViens { get; set; } = new List<ThanNhanNhanVien>();
    }
}
