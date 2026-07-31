using System.ComponentModel;

namespace PayrollManagementSystem.Domain.Enums
{
    public enum TrangThaiDonNghi
    {
        [Description("Chờ duyệt")] 
        CHO_DUYET,

        [Description("Đã duyệt")] 
        DA_DUYET,

        [Description("Từ chối")] 
        TU_CHOI,
    }
}
