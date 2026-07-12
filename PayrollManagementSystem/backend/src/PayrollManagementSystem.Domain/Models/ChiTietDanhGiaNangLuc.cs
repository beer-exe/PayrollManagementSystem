using PayrollManagementSystem.Domain.Common;
namespace PayrollManagementSystem.Domain.Models
{
    public class ChiTietDanhGiaNangLuc : BaseAuditableEntity
    {
        public Guid IdChiTiet { get; set; }
        public Guid IdPhieu { get; set; }
        public Guid IdTieuChi { get; set; }
        public int? DiemTuDanhGia { get; set; }
        public int? DiemQuanLyDanhGia { get; set; }
        public string? NhanXetNhanVien { get; set; }
        public string? NhanXetQuanLy { get; set; }

        public PhieuDanhGiaNangLuc PhieuDanhGia { get; set; } = null!;
        public KhungNangLucP2 TieuChi { get; set; } = null!;
    }
}
