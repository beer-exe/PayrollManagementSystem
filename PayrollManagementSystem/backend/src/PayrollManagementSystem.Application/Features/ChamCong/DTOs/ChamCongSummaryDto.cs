namespace PayrollManagementSystem.Application.Features.ChamCong.DTOs
{
    public class ChamCongSummaryDto
    {
        public string CccdNhanVien { get; set; } = null!;
        public string HoTenNhanVien { get; set; } = null!;
        public string? TenPhongBan { get; set; }
        public int Thang { get; set; }
        public int Nam { get; set; }
        public decimal NgayCongChuan { get; set; }
        public decimal TongNgayCongThucTe { get; set; }
        public decimal NgayNghiLe { get; set; }
        public decimal NgayNghiCuoiTuan { get; set; }
        public decimal NgayVangKhongPhep { get; set; }

    }
}
