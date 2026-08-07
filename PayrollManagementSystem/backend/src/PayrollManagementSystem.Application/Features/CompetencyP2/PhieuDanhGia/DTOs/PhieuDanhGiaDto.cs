using System;
using System.Collections.Generic;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Extensions;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.PhieuDanhGia.DTOs
{
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
        public string TenTrangThai => Enum.TryParse<TrangThaiPhieuDanhGia>(TrangThai, out var e) ? e.GetDescription() : TrangThai;
        public bool CanEvaluate { get; set; }
        public List<ChiTietDanhGiaDto> ChiTietDanhGias { get; set; } = new List<ChiTietDanhGiaDto>();
    }
}
