namespace PayrollManagementSystem.Application.Features.Employees.Commands.UpdateEmployee
{
    public class UpdateThanNhanDto
    {
        public string? MaDinhDanh { get; set; }
        public string TenTn { get; set; } = null!;
        public DateOnly? NgaySinh { get; set; }
        public Guid? IdMqh { get; set; }
        public bool LaNguoiPhuThuoc { get; set; }
    }
}
