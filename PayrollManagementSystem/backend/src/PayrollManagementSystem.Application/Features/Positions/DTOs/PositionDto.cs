namespace PayrollManagementSystem.Application.Features.Positions.DTOs
{
    public class PositionDto
    {
        public string IdChucVu { get; set; } = null!;
        public string TenChucVu { get; set; } = null!;
        public string? MoTaCongViec { get; set; }
        public string TrangThai { get; set; } = null!;
    }
}
