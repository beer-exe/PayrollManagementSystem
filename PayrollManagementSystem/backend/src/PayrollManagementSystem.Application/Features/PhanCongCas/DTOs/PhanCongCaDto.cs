using System;

namespace PayrollManagementSystem.Application.Features.PhanCongCas.DTOs
{
    public class PhanCongCaDto
    {
        public Guid IdPhanCong { get; set; }
        public string CccdNhanVien { get; set; } = null!;
        public DateOnly NgayLamViec { get; set; }
        public Guid? IdCaLamViec { get; set; }
        public string? TenCa { get; set; }
        public string HoTenNhanVien { get; set; } = null!;
        public string? GhiChu { get; set; }
    }
}
