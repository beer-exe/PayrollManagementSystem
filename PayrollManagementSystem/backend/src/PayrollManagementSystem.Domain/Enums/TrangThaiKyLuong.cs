using System.ComponentModel;

namespace PayrollManagementSystem.Domain.Enums
{
    public enum TrangThaiKyLuong
    {
        [Description("Chưa chốt")]
        CHUA_CHOT,

        [Description("Đã chốt")]
        DA_CHOT,

        [Description("Đã thanh toán")]
        DA_THANH_TOAN
    }
}
