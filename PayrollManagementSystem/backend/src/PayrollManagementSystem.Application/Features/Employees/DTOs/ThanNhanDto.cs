namespace PayrollManagementSystem.Application.Features.Profile.DTOs
{
    public class ThanNhanDto
    {
        public string? MaDinhDanh { get; set; }
        public string TenTn { get; set; } = null!;
        public string? NgaySinh { get; set; }
        public Guid? IdMqh { get; set; }
        public string? MoiQuanHe { get; set; }
        public bool LaNguoiPhuThuoc { get; set; }
    }
}