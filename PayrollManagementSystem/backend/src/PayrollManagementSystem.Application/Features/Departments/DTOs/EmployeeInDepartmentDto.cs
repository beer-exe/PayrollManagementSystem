namespace PayrollManagementSystem.Application.Features.Departments.DTOs
{
    public class EmployeeInDepartmentDto
    {
        public string Cccd { get; set; } = null!;
        public string HoTen { get; set; } = null!;
        public string? Email { get; set; }
        public string? TenChucVu { get; set; }
        public string? TrangThai { get; set; }
        public DateOnly? NgayVaoLam { get; set; }
    }
}