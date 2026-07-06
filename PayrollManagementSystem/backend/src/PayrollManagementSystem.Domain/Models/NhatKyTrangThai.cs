using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Domain.Models
{
    public class NhatKyTrangThai
    {
        public Guid IdNhatKy { get; set; }
        public string Cccd { get; set; } = null!;
        public TrangThaiNhanVien? TrangThaiCu { get; set; }
        public TrangThaiNhanVien TrangThaiMoi { get; set; }
        public string LyDo { get; set; } = null!;
        public DateTime NgayThayDoi { get; set; }
        public string NguoiThayDoi { get; set; } = null!;

        // Navigation properties
        public NhanVien NhanVien { get; set; } = null!;
    }
}