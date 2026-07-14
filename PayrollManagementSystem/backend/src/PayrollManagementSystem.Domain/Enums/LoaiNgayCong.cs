using System.ComponentModel;

namespace PayrollManagementSystem.Domain.Enums
{
    public enum LoaiNgayCong
    {
        [Description("Làm đủ ca")]
        LAM_DU_CA,

        [Description("Nửa ca")]
        NUA_CA,

        [Description("Đi trễ / Về sớm")]
        DI_TRE_VE_SOM,

        [Description("Vắng có phép")]
        VANG_CO_PHEP,

        [Description("Vắng không phép")]
        VANG_KHONG_PHEP,

        [Description("Nghỉ lễ")]
        NGHI_LE,

        [Description("Nghỉ cuối tuần")]
        NGHI_CUOI_TUAN,
    }
}
