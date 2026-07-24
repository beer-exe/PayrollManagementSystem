namespace PayrollManagementSystem.Application.Features.Payroll.Queries.GetPayrollList
{
    public class PayrollListDto
    {
        public Guid IdBangLuong { get; set; }
        public Guid IdKyLuong { get; set; }
        public string CccdNhanVien { get; set; } = null!;
        public string TenNhanVien { get; set; } = null!;
        public string TenPhongBan { get; set; } = null!;
        public string TenChucVu { get; set; } = null!;
        
        public int Thang { get; set; }
        public int Nam { get; set; }
        
        public decimal P1 { get; set; }
        public decimal HeSoP2 { get; set; }
        public decimal HeSoP3 { get; set; }
        
        public decimal NgayCongChuan { get; set; }
        public decimal NgayCongThucTe { get; set; }
        
        public decimal LuongThoiGian { get; set; }
        public decimal LuongHieuSuatP3 { get; set; }
        
        public decimal PhuCap { get; set; }
        public decimal Thuong { get; set; }
        public decimal TangCa { get; set; }
        
        public decimal Phat { get; set; }
        public decimal TruBaoHiem { get; set; }
        public decimal TruThue { get; set; }
        
        public decimal TongThuNhap { get; set; }
        public decimal ThucLinh { get; set; }
        
        public string? GhiChu { get; set; }
    }
}
