using System;

namespace PayrollManagementSystem.Application.Features.DonNghi.DTOs
{
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
