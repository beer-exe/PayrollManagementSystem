namespace PayrollManagementSystem.Application.Features.JobGrades.DTOs
{
    public class JobGradeDto
    {
        public string IdNgachLuong { get; set; } = null!;
        public string TenNgachLuong { get; set; } = null!;
        public string? MoTa { get; set; }
        public int TrangThai { get; set; }
    }
}
