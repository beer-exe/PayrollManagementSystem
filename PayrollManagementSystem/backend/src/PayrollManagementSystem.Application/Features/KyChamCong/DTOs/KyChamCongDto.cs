using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Application.Features.KyChamCong.DTOs
{
    public class KyChamCongDto
    {
        public Guid Id { get; set; }
        public int Thang { get; set; }
        public int Nam { get; set; }
        public string TrangThai { get; set; } = null!;
        public string TrangThaiText { get; set; } = null!;
    }
}
