using System.ComponentModel;

namespace PayrollManagementSystem.Domain.Enums
{
    public enum TrangThaiKyDanhGia
    {
        [Description("Khởi tạo")]
        KHOI_TAO = 0,

        [Description("Đang đánh giá")]
        DANG_DANH_GIA = 1,

        [Description("Đã chốt")]
        DA_CHOT = 2,

        [Description("Đã hủy")]
        DA_HUY = 3
    }
}
