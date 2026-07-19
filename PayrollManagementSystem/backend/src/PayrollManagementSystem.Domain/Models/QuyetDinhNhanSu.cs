using PayrollManagementSystem.Domain.Common;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Domain.Models
{
    public class QuyetDinhNhanSu : BaseAuditableEntity
    {
        public string SoQuyetDinh { get; set; } = null!;
        public string? Cccd { get; set; }
        public string LoaiQuyetDinh { get; set; } = null!;
        public string? IdBacLuongMoi { get; set; }
        public string? IdChucVuMoi { get; set; }
        public string? IdBacLuongCu { get; set; }
        public string? IdChucVuCu { get; set; }
        public DateOnly NgayHieuLuc { get; set; }
        public DateOnly? NgayHetHan { get; set; }
        public string? NguoiKy { get; set; }
        public TrangThaiQuyetDinh TrangThai { get; set; } = TrangThaiQuyetDinh.HIEU_LUC;

        // Navigation properties
        public NhanVien? NhanVien { get; set; }
        public BacLuong? BacLuong { get; set; }
        public ChucVu? ChucVuMoi { get; set; }
    }
}
