using System;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Extensions;

namespace PayrollManagementSystem.Application.Features.Profile.DTOs
{
    public class UserProfileDto
    {
        public string Cccd { get; set; } = null!;
        public string HoTen { get; set; } = null!;
        public bool? GioiTinh { get; set; }
        public string? Sdt { get; set; }
        public string? Email { get; set; }
        public DateOnly? NgaySinh { get; set; }
        public string? DanToc { get; set; }
        public string? DiaChi { get; set; }
        public string? ChuyenNganh { get; set; }
        public DateOnly? NgayVaoLam { get; set; }
        public string? TrangThai { get; set; }
        public string? TenTrangThai => TrangThai != null && Enum.TryParse<TrangThaiNhanVien>(TrangThai, out var e) ? e.GetDescription() : TrangThai;
        public string? SoBhxh { get; set; }
        public string? SoBhyt { get; set; }
        public string? TenPhongBan { get; set; }
        public string? TenChucVu { get; set; }
    }
}