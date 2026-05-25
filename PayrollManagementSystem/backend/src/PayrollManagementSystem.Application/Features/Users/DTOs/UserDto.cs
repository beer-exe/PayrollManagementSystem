namespace PayrollManagementSystem.Application.Features.Users.DTOs
{
    public class UserDto
    {
        public Guid IdTaiKhoan { get; set; }
        public string TenTaiKhoan { get; set; } = null!;
        public string? Email { get; set; }
        public string? HoTen { get; set; }
        public string? TenVaiTro { get; set; }
        public Guid? IdVaiTro { get; set; }
        public string TrangThai { get; set; } = null!;
    }
}