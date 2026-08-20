namespace PayrollManagementSystem.Application.Features.WorkSchedule.DTOs
{
    public class ChiTietLichLamViecDto
    {
        public Guid Id { get; set; }
        public DateOnly Ngay { get; set; }
        public string Thu { get; set; } = null!;
        public string LoaiNgay { get; set; } = null!;
        public string? TenNgayNghi { get; set; }
        public decimal SoGioLam { get; set; }
        public Guid? IdCaLamViecMacDinh { get; set; }
        public string? TenCaLamViecMacDinh { get; set; }
    }
}
