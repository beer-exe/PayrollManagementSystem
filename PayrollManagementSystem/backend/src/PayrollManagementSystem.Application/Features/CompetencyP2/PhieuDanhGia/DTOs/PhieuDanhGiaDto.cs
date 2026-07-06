namespace PayrollManagementSystem.Application.Features.CompetencyP2.PhieuDanhGia.DTOs
{
    public class ChiTietDanhGiaDto
    {
        public Guid IdChiTiet { get; set; }
        public Guid IdTieuChi { get; set; }
        public string TenNangLuc { get; set; } = null!;
        public string YeuCauToiThieu { get; set; } = null!;
        public decimal TyTrong { get; set; }
        public int? DiemTuDanhGia { get; set; }
        public int? DiemQuanLyDanhGia { get; set; }
        public string? NhanXetNhanVien { get; set; }
        public string? NhanXetQuanLy { get; set; }
    }

    public class PhieuDanhGiaDto
    {
        public Guid IdPhieu { get; set; }
        public Guid IdKyDanhGia { get; set; }
        public string TenKyDanhGia { get; set; } = null!;
        public string CccdNhanVien { get; set; } = null!;
        public decimal? DiemTongHop { get; set; }
        public decimal? HeSoP2 { get; set; }
        public string? XepLoai { get; set; }
        public string? NhanXetChung { get; set; }
        public string TrangThai { get; set; } = null!;
        public bool CanEvaluate { get; set; }
        public List<ChiTietDanhGiaDto> ChiTietDanhGias { get; set; } = new List<ChiTietDanhGiaDto>();
    }
}
