using System.ComponentModel;

namespace PayrollManagementSystem.Domain.Enums
{
    public enum TrangThaiChamCong
    {
        [Description("Chưa xác nhận")]
        CHUA_XAC_NHAN,

        [Description("Đã xác nhận")]
        DA_XAC_NHAN,

        [Description("Cần giải trình")]
        CAN_GIAI_TRINH,
    }
}
