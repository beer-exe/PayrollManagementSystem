namespace PayrollManagementSystem.Domain.Models
{
    public class MucQuyDoiP2
    {
        public Guid IdQuyDoi { get; set; }
        public string XepLoai { get; set; } = null!;
        public decimal DiemToiThieu { get; set; }
        public decimal DiemToiDa { get; set; }
        public decimal HeSoP2 { get; set; }
    }
}
