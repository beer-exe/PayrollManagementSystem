using System.ComponentModel;

namespace PayrollManagementSystem.Domain.Enums
{
    public enum LoaiNgay
    {
        [Description("Ngày làm việc")]
        NGAY_LAM_VIEC,

        [Description("Nghỉ cuối tuần")]
        NGHI_CUOI_TUAN,

        [Description("Nghỉ lễ")]
        NGHI_LE
    }
}
