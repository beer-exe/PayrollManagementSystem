using PayrollManagementSystem.Domain.Common;

namespace PayrollManagementSystem.Domain.Models
{
    public class CauHinhGiamTru : BaseAuditableEntity
    {
        public Guid IdCauHinhGiamTru { get; set; } = Guid.NewGuid();

        /// <summary>M?c gi?m tr? b?n thân (VNÐ/tháng). M?c d?nh: 11,000,000</summary>
        public decimal GiamTruBanThan { get; set; } = 11_000_000m;

        /// <summary>M?c gi?m tr? m?i ngu?i ph? thu?c (VNÐ/tháng). M?c d?nh: 4,400,000</summary>
        public decimal GiamTruNguoiPhuThuoc { get; set; } = 4_400_000m;

        public bool IsActive { get; set; } = true;

        public string? GhiChu { get; set; }
    }
}
