using PayrollManagementSystem.Domain.Common;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Domain.Models
{
    public class KyChamCong : BaseAuditableEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int Thang { get; set; }
        public int Nam { get; set; }
        public TrangThaiKyChamCong TrangThai { get; set; } = TrangThaiKyChamCong.DANG_MO;

        public ICollection<ChamCong> ChamCongs { get; set; } = new List<ChamCong>();
    }
}
