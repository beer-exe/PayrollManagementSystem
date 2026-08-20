using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Extensions;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.KyDanhGia.DTOs
{
    public class KyDanhGiaDto
    {
        public Guid IdKyDanhGia { get; set; }
        public string TenKyDanhGia { get; set; } = null!;
        public int Nam { get; set; }
        public DateOnly NgayBatDau { get; set; }
        public DateOnly NgayKetThuc { get; set; }
        public TrangThaiKyDanhGia TrangThai { get; set; }
        public string TenTrangThai => TrangThai.GetDescription();
    }
}
