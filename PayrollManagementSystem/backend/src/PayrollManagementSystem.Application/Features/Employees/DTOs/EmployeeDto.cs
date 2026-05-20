using PayrollManagementSystem.Application.Features.Profile.DTOs;

namespace PayrollManagementSystem.Application.Features.Employees.DTOs
{
    public class EmployeeDto
    {
        public string Cccd { get; set; } = null!;
        public string HoTen { get; set; } = null!;
        public bool? GioiTinh { get; set; }
        public string? Sdt { get; set; }
        public string? Email { get; set; }
        public string? NgaySinh { get; set; }
        public string? DanToc { get; set; }
        public string? DiaChi { get; set; }
        public string? ChuyenNganh { get; set; }
        public string? NgayVaoLam { get; set; }
        public string? TrangThai { get; set; }
        public string? SoBhxh { get; set; }
        public string? SoBhyt { get; set; }
        public string? TenPhongBan { get; set; }
        public string? TenChucVu { get; set; }
        public List<ThanNhanDto> ThanNhans { get; set; } = new List<ThanNhanDto>();
    }
}