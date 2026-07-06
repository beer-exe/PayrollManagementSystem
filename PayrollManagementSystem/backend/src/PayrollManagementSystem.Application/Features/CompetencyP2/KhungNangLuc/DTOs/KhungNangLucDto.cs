using System;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.KhungNangLuc.DTOs
{
    public class KhungNangLucDto
    {
        public Guid IdTieuChi { get; set; }
        public string IdChucVu { get; set; } = null!;
        public string TenNangLuc { get; set; } = null!;
        public string YeuCauToiThieu { get; set; } = null!;
        public decimal TyTrong { get; set; }
    }
}
