namespace PayrollManagementSystem.Application.Features.ChamCong.DTOs
{
    public class ChamCongDto
    {
        public Guid Id { get; set; }
        public string CccdNhanVien { get; set; } = null!;
        public string HoTenNhanVien { get; set; } = null!;
        public string NgayChamCong { get; set; } = null!;
        public string? GioVao { get; set; }
        public string? GioRa { get; set; }
        public decimal SoGioLamThucTe { get; set; }
        public decimal SoNgayCong { get; set; }
        public string LoaiNgayCong { get; set; } = null!;
        public string TrangThai { get; set; } = null!;
        public bool IsNhapTay { get; set; }
        public string? GhiChu { get; set; }
        public DateTime? NgayTao { get; set; }
        public string? NguoiTao { get; set; }
    }
}
