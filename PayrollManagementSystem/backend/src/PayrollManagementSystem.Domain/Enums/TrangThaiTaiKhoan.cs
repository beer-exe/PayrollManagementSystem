using System.ComponentModel;

namespace PayrollManagementSystem.Domain.Enums
{
    public enum TrangThaiTaiKhoan
    {
        [Description("Hoạt động")]
        HOAT_DONG,

        [Description("Khóa")]
        KHOA,

        [Description("Chờ xác nhận")]
        CHO_XAC_NHAN
    }
}
