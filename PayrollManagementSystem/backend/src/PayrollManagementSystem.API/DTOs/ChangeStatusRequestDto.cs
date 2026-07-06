using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.API.DTOs
{
    public class ChangeStatusRequestDto
    {
        public TrangThaiNhanVien TrangThaiMoi { get; set; }
        public string LyDo { get; set; } = null!;
    }
}
