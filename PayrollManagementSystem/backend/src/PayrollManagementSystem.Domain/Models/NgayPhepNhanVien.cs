using PayrollManagementSystem.Domain.Common;

namespace PayrollManagementSystem.Domain.Models
{
    public class NgayPhepNhanVien : BaseAuditableEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>CCCD nhân viên — FK → NhanVien.Cccd</summary>
        public string CccdNhanVien { get; set; } = null!;

        public int Nam { get; set; }

        /// <summary>Tổng số ngày phép được cấp trong năm (mặc định 12 theo luật)</summary>
        public decimal TongNgayPhep { get; set; } = 12;

        /// <summary>Số ngày phép đã sử dụng (cộng dồn khi đơn NGHI_PHEP_NAM được duyệt)</summary>
        public decimal DaSuDung { get; set; } = 0;

        /// <summary>Số ngày phép còn lại = TongNgayPhep - DaSuDung</summary>
        public decimal ConLai => TongNgayPhep - DaSuDung;

        // Navigation properties
        public NhanVien NhanVien { get; set; } = null!;
    }
}
