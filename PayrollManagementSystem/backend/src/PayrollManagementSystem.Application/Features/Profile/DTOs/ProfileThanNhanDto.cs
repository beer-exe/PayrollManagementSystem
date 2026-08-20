namespace PayrollManagementSystem.Application.Features.Profile.DTOs
{
    public class ProfileThanNhanDto
    {
        public string? MaDinhDanh { get; set; }
        public string TenTn { get; set; } = null!;
        public DateOnly? NgaySinh { get; set; }
        public string? MoiQuanHe { get; set; }
    }
}
