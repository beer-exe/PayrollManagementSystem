using System;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Extensions;

namespace PayrollManagementSystem.Application.Features.Profile.DTOs
{
    public class LichSuCongTacDto
    {
        public string SoQuyetDinh { get; set; } = null!;
        public string LoaiQuyetDinh { get; set; } = null!;
        public DateOnly NgayHieuLuc { get; set; }
        public string? TenPhongBanMoi { get; set; }
        public string? TenChucVuMoi { get; set; }
        public decimal? LuongP1Moi { get; set; }
        public string TrangThai { get; set; } = null!;
        public string? TenTrangThai => Enum.TryParse<TrangThaiQuyetDinh>(TrangThai, out var e) ? e.GetDescription() : TrangThai;
    }
}
