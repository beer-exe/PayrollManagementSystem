using PayrollManagementSystem.Domain.Common;
namespace PayrollManagementSystem.Domain.Models
{
    public class KhungNangLucP2 : BaseAuditableEntity
    {
        public Guid IdTieuChi { get; set; }
        public string IdChucVu { get; set; } = null!;
        public string TenNangLuc { get; set; } = null!;
        public string? MoTa { get; set; }
        public decimal TyTrong { get; set; }

        public ChucVu ChucVu { get; set; } = null!;
    }
}
