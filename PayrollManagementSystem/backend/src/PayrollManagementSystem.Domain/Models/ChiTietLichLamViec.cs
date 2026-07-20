using PayrollManagementSystem.Domain.Common;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Domain.Models
{
    public class ChiTietLichLamViec : BaseAuditableEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid IdLich { get; set; }
        public DateOnly Ngay { get; set; }
        public string Thu { get; set; } = null!;
        public LoaiNgay LoaiNgay { get; set; }
        public string? TenNgayNghi { get; set; }
        public decimal SoGioLam { get; set; } = 8;
        public Guid? IdCaLamViecMacDinh { get; set; }

        // Navigation property
        public LichLamViec LichLamViec { get; set; } = null!;
        public CaLamViec? CaLamViecMacDinh { get; set; }
    }
}
