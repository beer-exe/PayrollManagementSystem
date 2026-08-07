using System;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.PhieuDanhGia.Commands.SubmitTuDanhGia
{
    public class ChiTietTuDanhGiaDto
    {
        public Guid IdChiTiet { get; set; }
        public int DiemTuDanhGia { get; set; }
        public string? NhanXetNhanVien { get; set; }
    }
}
