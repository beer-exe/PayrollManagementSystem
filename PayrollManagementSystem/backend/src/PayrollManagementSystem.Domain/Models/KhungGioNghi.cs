using PayrollManagementSystem.Domain.Common;

namespace PayrollManagementSystem.Domain.Models
{
    public class KhungGioNghi : BaseAuditableEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? IdCaLamViec { get; set; }
        public string TenKhoangNghi { get; set; } = null!;
        public TimeSpan GioBatDau { get; set; }
        public TimeSpan GioKetThuc { get; set; }
        public bool TinhVaoGioLam { get; set; } = false;

        // Navigation properties
        public CaLamViec? CaLamViec { get; set; }
    }
}
