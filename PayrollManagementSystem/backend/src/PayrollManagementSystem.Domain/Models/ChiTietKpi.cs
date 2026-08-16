using PayrollManagementSystem.Domain.Common;

namespace PayrollManagementSystem.Domain.Models
{
    public class ChiTietKpi : BaseAuditableEntity
    {
        public Guid IdChiTietKpi { get; set; }
        public Guid IdPhieuKpi { get; set; }
        public string MucTieu { get; set; } = null!;
        public string DonViTinh { get; set; } = null!;
        
        public decimal TrongSo { get; set; } = 0m;
        public decimal ChiTieu { get; set; } = 0m;
        public decimal ThucTe { get; set; } = 0m;
        public decimal TiLeHoanThanh { get; set; } = 0m;
        public decimal DiemKpi { get; set; } = 0m;

        public PhieuKpi PhieuKpi { get; set; } = null!;
    }
}
