using PayrollManagementSystem.Domain.Common;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Domain.Models
{
    public class PhieuKpi : BaseAuditableEntity
    {
        public Guid IdPhieuKpi { get; set; }
        public Guid IdKyKpi { get; set; }
        public string CccdNhanVien { get; set; } = null!;
        public string? CccdQuanLy { get; set; }
        public decimal TongDiemKpi { get; set; } = 0m;
        public decimal HeSoP3 { get; set; } = 1.0m;
        public string? NhanXet { get; set; }
        public TrangThaiPhieuKpi TrangThai { get; set; } = TrangThaiPhieuKpi.CHO_GIAO_MUC_TIEU;

        public KyKpi KyKpi { get; set; } = null!;
        public NhanVien NhanVien { get; set; } = null!;
        public NhanVien? QuanLy { get; set; }
        public ICollection<ChiTietKpi> ChiTietKpis { get; set; } = new List<ChiTietKpi>();
    }
}
