namespace PayrollManagementSystem.Application.Features.WorkSchedule.DTOs
{
    public class MyScheduleDayDto
    {
        public DateOnly Ngay { get; set; }
        public string Thu { get; set; } = null!;
        public string LoaiNgay { get; set; } = null!; // Ngày làm việc, Nghỉ cuối tuần, Nghỉ lễ, ...
        public string? TenNgayNghi { get; set; }

        public Guid? IdCaLamViec { get; set; }
        public string? TenCa { get; set; }
        public TimeSpan? GioBatDau { get; set; }
        public TimeSpan? GioKetThuc { get; set; }
        public bool XuyenNgay { get; set; }

        public bool LaCaDuocPhanCong { get; set; } // true nếu là ca riêng, false nếu là ca mặc định

        public bool CoNghiPhep { get; set; }
        public string? LoaiNghiPhep { get; set; }
    }
}
