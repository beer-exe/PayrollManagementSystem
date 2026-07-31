using PayrollManagementSystem.Domain.Common;

namespace PayrollManagementSystem.Domain.Models
{
    public class BangLuong : BaseAuditableEntity
    {
        public Guid IdBangLuong { get; set; } = Guid.NewGuid();
        public Guid IdKyLuong { get; set; }
        public string CccdNhanVien { get; set; } = null!;
        
        public int Thang { get; set; }
        public int Nam { get; set; }
        
        // Cấu phần Lương 3P
        public decimal P1 { get; set; }
        public decimal HeSoP2 { get; set; } = 1.0m;
        public decimal HeSoP3 { get; set; } = 1.0m;
        
        // Thời gian làm việc
        public decimal NgayCongChuan { get; set; }
        public decimal NgayCongThucTe { get; set; }
        public decimal GioCongChuan { get; set; }
        public decimal GioCongThucTe { get; set; }
        
        // Các khoản thu nhập
        public decimal LuongThoiGian { get; set; }
        public decimal LuongHieuSuatP3 { get; set; }
        public decimal PhuCap { get; set; } = 0m;
        public decimal Thuong { get; set; } = 0m;
        public decimal TangCa { get; set; } = 0m;
        
        // Các khoản trừ
        public decimal Phat { get; set; } = 0m;
        public decimal KhauTru { get; set; } = 0m;
        public decimal TruThue { get; set; } = 0m;
        
        // Tổng hợp
        public decimal TongThuNhap { get; set; }
        public decimal ThucLinh { get; set; }

        // Ghi chú (ví dụ diễn giải công thức nếu cần)
        public string? GhiChu { get; set; }
        
        // Chi tiết khoản mục khấu trừ (lưu trữ JSON)
        public string? ChiTietKhauTru { get; set; }

        // Navigation properties
        public KyLuong KyLuong { get; set; } = null!;
        public NhanVien NhanVien { get; set; } = null!;
    }
}
