using PayrollManagementSystem.Domain.Common;
namespace PayrollManagementSystem.Domain.Models
{
    public class MucQuyDoiP2 : BaseAuditableEntity
    {
        public Guid IdQuyDoi { get; set; }
        public string XepLoai { get; set; } = null!;
        public decimal DiemToiThieu { get; set; }
        public decimal DiemToiDa { get; set; }
        public decimal HeSoP2 { get; set; }
    }
}
