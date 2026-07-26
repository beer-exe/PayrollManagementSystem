using System;
using PayrollManagementSystem.Domain.Common;

namespace PayrollManagementSystem.Domain.Models
{
    public class PhanCongCa : BaseAuditableEntity
    {
        public Guid IdPhanCong { get; set; } = Guid.NewGuid();
        public string CccdNhanVien { get; set; } = null!;
        public DateOnly NgayLamViec { get; set; }
        
        public Guid? IdCaLamViec { get; set; }

        public string? GhiChu { get; set; }

        // Navigation properties
        public NhanVien NhanVien { get; set; } = null!;
        public CaLamViec? CaLamViec { get; set; }
    }
}
