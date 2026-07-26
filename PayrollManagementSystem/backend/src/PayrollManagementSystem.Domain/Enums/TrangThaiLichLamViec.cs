using System.ComponentModel;

namespace PayrollManagementSystem.Domain.Enums
{
    public enum TrangThaiLichLamViec
    {
        [Description("Hiệu lực")]
        HIEU_LUC,

        [Description("Đã hủy")]
        DA_HUY,

        [Description("Hết hiệu lực")]
        HET_HIEU_LUC
    }
}
