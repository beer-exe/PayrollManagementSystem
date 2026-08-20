namespace PayrollManagementSystem.Application.Features.WorkSchedule.DTOs
{
    public class LichLamViecDto
    {
        public Guid IdLich { get; set; }
        public int Nam { get; set; }
        public string TrangThai { get; set; } = null!;
        public int TongNgayLam { get; set; }
        public int TongNgayNghiCuoiTuan { get; set; }
        public int TongNgayLe { get; set; }
        public int TongNgay { get; set; }
        public string? GhiChu { get; set; }
        public string? NguoiTao { get; set; }
        public DateTime? NgayTao { get; set; }
    }
}
