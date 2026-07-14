namespace PayrollManagementSystem.Application.Features.DonNghi.DTOs
{
    public class DonNghiDto
    {
        public Guid Id { get; set; }
        public string CccdNhanVien { get; set; } = null!;
        public string HoTenNhanVien { get; set; } = null!;
        public string? TenPhongBan { get; set; }
        public string LoaiNghi { get; set; } = null!;      // GetDescription()
        public string NgayBatDau { get; set; } = null!;    // "yyyy-MM-dd"
        public string NgayKetThuc { get; set; } = null!;
        public decimal SoNgayNghi { get; set; }
        public string LyDo { get; set; } = null!;
        public string? TaiLieuDinhKem { get; set; }
        public string TrangThai { get; set; } = null!;     // GetDescription()
        public string? HoTenNguoiDuyet { get; set; }
        public string? LyDoTuChoi { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public DateTime? NgayTao { get; set; }
    }

    public class NgayPhepDto
    {
        public Guid Id { get; set; }
        public string CccdNhanVien { get; set; } = null!;
        public string HoTenNhanVien { get; set; } = null!;
        public string? TenPhongBan { get; set; }
        public int Nam { get; set; }
        public decimal TongNgayPhep { get; set; }
        public decimal DaSuDung { get; set; }
        public decimal ConLai { get; set; }
    }
}
