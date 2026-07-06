using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Domain.Models
{
    public class PhieuDanhGiaNangLuc
    {
        public Guid IdPhieu { get; set; }
        public Guid IdKyDanhGia { get; set; }
        public string CccdNhanVien { get; set; } = null!;
        public string? CccdQuanLy { get; set; }
        public decimal? DiemTongHop { get; set; }
        public decimal? HeSoP2 { get; set; }
        public string? XepLoai { get; set; }
        public string? NhanXetChung { get; set; }
        public TrangThaiPhieuDanhGia TrangThai { get; set; } = TrangThaiPhieuDanhGia.CHO_NV_DANH_GIA;

        public KyDanhGia KyDanhGia { get; set; } = null!;
        public NhanVien NhanVien { get; set; } = null!;
        public NhanVien? QuanLy { get; set; }
        public ICollection<ChiTietDanhGiaNangLuc> ChiTietDanhGias { get; set; } = new List<ChiTietDanhGiaNangLuc>();
    }
}
