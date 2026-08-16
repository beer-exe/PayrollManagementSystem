using PayrollManagementSystem.Domain.Common;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Domain.Models
{
    public class KyKpi : BaseAuditableEntity
    {
        public Guid IdKyKpi { get; set; }
        public string TenKyKpi { get; set; } = null!;
        public int Thang { get; set; }
        public int Nam { get; set; }
        public TrangThaiKyKpi TrangThai { get; set; } = TrangThaiKyKpi.KHOI_TAO;

        public ICollection<PhieuKpi> PhieuKpis { get; set; } = new List<PhieuKpi>();
    }
}
