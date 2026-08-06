using System;

namespace PayrollManagementSystem.Application.Features.ThueTncn.DTOs
{
    public class CauHinhGiamTruDto
    {
        public Guid? IdCauHinhGiamTru { get; set; }
        public decimal GiamTruBanThan { get; set; }
        public decimal GiamTruNguoiPhuThuoc { get; set; }
        public string? GhiChu { get; set; }
    }
}
