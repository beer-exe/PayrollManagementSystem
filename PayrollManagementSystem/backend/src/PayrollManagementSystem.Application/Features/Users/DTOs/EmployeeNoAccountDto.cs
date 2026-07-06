namespace PayrollManagementSystem.Application.Features.Users.DTOs
{
    public class EmployeeNoAccountDto
    {
        public string Cccd { get; set; } = null!;
        public string HoTen { get; set; } = null!;
        public string? Email { get; set; }
        public string? TenPhongBan { get; set; }
    }
}
