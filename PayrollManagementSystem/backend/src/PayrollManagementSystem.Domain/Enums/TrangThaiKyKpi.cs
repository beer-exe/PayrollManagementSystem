using System.ComponentModel;

namespace PayrollManagementSystem.Domain.Enums
{
    public enum TrangThaiKyKpi
    {
        [Description("Khởi tạo")]
        KHOI_TAO = 0,

        [Description("Đang thực hiện")]
        DANG_THUC_HIEN = 1,

        [Description("Chờ phê duyệt")]
        CHO_PHE_DUYET = 2,

        [Description("Đã chốt")]
        DA_CHOT = 3,

        [Description("Đã hủy")]
        DA_HUY = 4
    }
}
