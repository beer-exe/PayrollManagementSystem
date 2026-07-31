using PayrollManagementSystem.Domain.Common;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Domain.Models
{
    public class KhoanKhauTru : BaseAuditableEntity
    {
        public Guid IdKhoanKhauTru { get; set; } = Guid.NewGuid();

        public string TenKhoanKhauTru { get; set; } = null!;
        public LoaiCongThucKhauTru LoaiCongThuc { get; set; }

        public decimal GiaTri { get; set; }
        public string? GhiChu { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
