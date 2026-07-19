namespace PayrollManagementSystem.Application.Features.WorkShifts.DTOs
{
    public class CaLamViecDto
    {
        public Guid Id { get; set; }
        public string TenCa { get; set; } = null!;
        public string GioBatDau { get; set; } = null!;
        public string GioKetThuc { get; set; } = null!;
        public bool XuyenNgay { get; set; }
        public decimal HeSoLuong { get; set; }
        public bool TrangThai { get; set; }

        public List<KhungGioNghiDto> KhungGioNghis { get; set; } = new List<KhungGioNghiDto>();
    }
}
