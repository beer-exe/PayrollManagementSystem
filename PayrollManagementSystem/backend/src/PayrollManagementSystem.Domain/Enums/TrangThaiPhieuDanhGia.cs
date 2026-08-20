using System.ComponentModel;

namespace PayrollManagementSystem.Domain.Enums
{
    public enum TrangThaiPhieuDanhGia
    {
        [Description("Chờ nhân viên đánh giá")]
        CHO_NV_DANH_GIA,

        [Description("Chờ quản lý đánh giá")]
        CHO_QL_DANH_GIA,

        [Description("Đã hoàn thành")]
        DA_HOAN_THANH,

        [Description("Đã hủy")]
        DA_HUY
    }
}
