using PayrollManagementSystem.Domain.Common;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Domain.Models
{
    public class KyLuong : BaseAuditableEntity
    {
        public Guid IdKyLuong { get; set; } = Guid.NewGuid();
        public int Thang { get; set; }
        public int Nam { get; set; }
        public string TenKyLuong { get; set; } = null!;
        public DateOnly NgayBatDau { get; set; }
        public DateOnly NgayKetThuc { get; set; }
        
        public TrangThaiKyLuong TrangThai { get; set; } = TrangThaiKyLuong.CHUA_CHOT;
        
        // Lý do mở chốt kỳ lương (Lưu vết kiểm toán)
        public string? LyDoMoChot { get; set; }

        // Navigation property
        public ICollection<BangLuong> BangLuongs { get; set; } = new List<BangLuong>();
    }
}
