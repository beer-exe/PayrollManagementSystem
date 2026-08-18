using System.ComponentModel;

namespace PayrollManagementSystem.Domain.Enums
{
    public enum TrangThaiBangLuong
    {
        [Description("Chưa xác nhận")]
        CHUA_XAC_NHAN = 0,

        [Description("Đã xác nhận")]
        DA_XAC_NHAN = 1,

        [Description("Yêu cầu xem xét")]
        YEU_CAU_XEM_XET = 2
    }
}
