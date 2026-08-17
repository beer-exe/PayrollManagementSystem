namespace PayrollManagementSystem.Application.Features.Payroll.Queries.GetKyLuongStatus
{
    public class KyLuongStatusDto
    {
        public int Thang { get; set; }
        public int Nam { get; set; }
        public string TenKyLuong { get; set; } = string.Empty;
        public DateOnly NgayBatDau { get; set; }
        public DateOnly NgayKetThuc { get; set; }
        public string TrangThai { get; set; } = "CHUA_TAO";
        public string TenTrangThai { get; set; } = "Chưa tạo";
        public bool IsLocked { get; set; }
        public bool CoDuLieuBangLuong { get; set; }
    }
}
