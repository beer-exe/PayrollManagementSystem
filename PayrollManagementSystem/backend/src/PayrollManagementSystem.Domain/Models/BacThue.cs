using PayrollManagementSystem.Domain.Common;

namespace PayrollManagementSystem.Domain.Models
{
    public class BacThue : BaseAuditableEntity
    {
        public Guid IdBacThue { get; set; } = Guid.NewGuid();

        /// <summary>S? th? t? b?c (1, 2, 3...)</summary>
        public int Bac { get; set; }

        /// <summary>Gi?i h?n du?i c?a b?c (VNÐ)</summary>
        public decimal TuGia { get; set; }

        /// <summary>Gi?i h?n trên c?a b?c (null = không gi?i h?n, t?c b?c cao nh?t)</summary>
        public decimal? DenGia { get; set; }

        /// <summary>Thu? su?t (%), ví d?: 5, 10, 15...</summary>
        public decimal ThueSuat { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
