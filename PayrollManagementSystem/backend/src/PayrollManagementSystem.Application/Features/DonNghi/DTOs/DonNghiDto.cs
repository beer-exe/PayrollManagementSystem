using System;

namespace PayrollManagementSystem.Application.Features.DonNghi.DTOs
{
    public class DonNghiDto
    {
        public Guid Id { get; set; }
        public string CccdNhanVien { get; set; } = null!;
        public string HoTenNhanVien { get; set; } = null!;
        public string? TenPhongBan { get; set; }
        public string LoaiNghi { get; set; } = null!;
        public string NgayBatDau { get; set; } = null!;
        public string NgayKetThuc { get; set; } = null!;
        public decimal SoNgayNghi { get; set; }
        public string LyDo { get; set; } = null!;
        public string? TaiLieuDinhKem { get; set; }
        public string TrangThai { get; set; } = null!;
        public string? HoTenNguoiDuyet { get; set; }
        public string? LyDoTuChoi { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public DateTime? NgayTao { get; set; }
    }
}
