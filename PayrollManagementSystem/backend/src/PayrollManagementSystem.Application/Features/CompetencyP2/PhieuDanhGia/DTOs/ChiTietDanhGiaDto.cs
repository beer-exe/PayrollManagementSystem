using System;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.PhieuDanhGia.DTOs
{
    public class ChiTietDanhGiaDto
    {
        public Guid IdChiTiet { get; set; }
        public Guid IdTieuChi { get; set; }
        public string TenNangLuc { get; set; } = null!;
        public string? MoTa { get; set; }
        public decimal TyTrong { get; set; }
        public int? DiemTuDanhGia { get; set; }
        public int? DiemQuanLyDanhGia { get; set; }
        public string? NhanXetNhanVien { get; set; }
        public string? NhanXetQuanLy { get; set; }
    }
}
