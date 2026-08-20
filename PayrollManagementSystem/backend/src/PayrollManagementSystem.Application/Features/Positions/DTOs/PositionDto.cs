using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Extensions;

namespace PayrollManagementSystem.Application.Features.Positions.DTOs
{
    public class PositionDto
    {
        public string IdChucVu { get; set; } = null!;
        public string TenChucVu { get; set; } = null!;
        public string? MoTaCongViec { get; set; }
        public string? IdNgachLuong { get; set; }
        public string? TenNgachLuong { get; set; }
        public string TrangThai { get; set; } = null!;
        public string TenTrangThai => Enum.TryParse<TrangThaiChucVu>(TrangThai, out var e) ? e.GetDescription() : TrangThai;
        public string IdPhongBan { get; set; } = null!;
        public string? TenPhongBan { get; set; }
        public string? IdChucVuQuanLy { get; set; }
        public string? TenChucVuQuanLy { get; set; }
    }
}
