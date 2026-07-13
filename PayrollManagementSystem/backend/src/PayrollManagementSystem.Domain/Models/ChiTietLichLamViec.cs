using PayrollManagementSystem.Domain.Common;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Domain.Models
{
    /// <summary>
    /// Chi tiết từng ngày trong lịch làm việc (3NF: mỗi bản ghi là 1 ngày, phụ thuộc duy nhất vào PK)
    /// </summary>
    public class ChiTietLichLamViec : BaseAuditableEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid IdLich { get; set; }
        public DateOnly Ngay { get; set; }
        public string Thu { get; set; } = null!;
        public LoaiNgay LoaiNgay { get; set; }
        public string? TenNgayNghi { get; set; }
        public decimal SoGioLam { get; set; } = 8;

        // Navigation property
        public LichLamViec LichLamViec { get; set; } = null!;
    }
}
