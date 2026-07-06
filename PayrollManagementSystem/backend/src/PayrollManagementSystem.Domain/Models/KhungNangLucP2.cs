namespace PayrollManagementSystem.Domain.Models
{
    public class KhungNangLucP2
    {
        public Guid IdTieuChi { get; set; }
        public string IdChucVu { get; set; } = null!;
        public string TenNangLuc { get; set; } = null!;
        public string YeuCauToiThieu { get; set; } = null!;
        public decimal TyTrong { get; set; }

        public ChucVu ChucVu { get; set; } = null!;
    }
}
