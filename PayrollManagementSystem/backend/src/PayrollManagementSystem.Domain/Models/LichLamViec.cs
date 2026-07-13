using PayrollManagementSystem.Domain.Common;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Domain.Models
{
    /// <summary>
    /// Lịch làm việc theo năm - tiêu đề lịch (3NF: chỉ lưu thông tin chung của năm)
    /// </summary>
    public class LichLamViec : BaseAuditableEntity
    {
        public Guid IdLich { get; set; } = Guid.NewGuid();
        public int Nam { get; set; }
        public TrangThaiLichLamViec TrangThai { get; set; } = TrangThaiLichLamViec.HIEU_LUC;
        public string? GhiChu { get; set; }

        // Navigation properties
        public ICollection<ChiTietLichLamViec> ChiTietLichLamViecs { get; set; } = new List<ChiTietLichLamViec>();
    }
}
