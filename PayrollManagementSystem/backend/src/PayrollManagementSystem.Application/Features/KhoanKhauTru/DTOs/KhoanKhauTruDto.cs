namespace PayrollManagementSystem.Application.Features.KhoanKhauTru.DTOs
{
    public class KhoanKhauTruDto
    {
        public Guid IdKhoanKhauTru { get; set; }
        public string TenKhoanKhauTru { get; set; } = null!;

        public string LoaiCongThuc { get; set; } = null!;

        public decimal GiaTri { get; set; }
        public string? GhiChu { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
