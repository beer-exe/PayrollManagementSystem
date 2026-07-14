namespace PayrollManagementSystem.Application.Features.ChamCong.DTOs
{
    public class ChamCongDto
    {
        public Guid Id { get; set; }
        public string CccdNhanVien { get; set; } = null!;
        public string HoTenNhanVien { get; set; } = null!;
        public string NgayChamCong { get; set; } = null!;   // "YYYY-MM-DD"
        public string? GioVao { get; set; }                  // "HH:mm"
        public string? GioRa { get; set; }                   // "HH:mm"
        public decimal SoGioLamThucTe { get; set; }
        public decimal SoNgayCong { get; set; }
        public string LoaiNgayCong { get; set; } = null!;   // Description từ enum
        public string TrangThai { get; set; } = null!;       // Description từ enum
        public bool IsNhapTay { get; set; }
        public string? GhiChu { get; set; }
        public DateTime? NgayTao { get; set; }
        public string? NguoiTao { get; set; }
    }

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
        public decimal NgayCanGiaiTrinh { get; set; }
    }

    public class ImportChamCongResultDto
    {
        public int TongSoDong { get; set; }
        public int ThanhCong { get; set; }
        public int ThatBai { get; set; }
        public List<string> LoiNhap { get; set; } = new();
    }
}
